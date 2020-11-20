using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MLocker.Core.Models;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IMusicService
    {
        Task<List<Song>> GetAllSongs();
        Task<string> GetSongUrl(int fileID);
        Task<bool> Upload(string fileName, Stream stream);
        Task<List<Album>> GetAlbums();
        Task IncrementPlayCount(int fileID);
        Task<List<string>> GetAllArtists();
        Task<Tuple<List<Album>, List<Song>>> GetArtistDetail(string artist);
        Task UpdateSongs();
        Task DownloadSongList(IEnumerable<Song> songs);
        Task DeleteCache();
        Task<bool> IsAllSongsCached(IEnumerable<Song> songs);
        Task RemoveSongsFromCache(IEnumerable<Song> songs);
        Task ScrollReset();
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private readonly IUploadRepository _uploadRepository;
        private readonly ISpinnerService _spinnerService;
        private readonly IJSRuntime _jsRuntime;
        private static List<Album> _albums;
        private static List<string> _artists;
        private static SemaphoreSlim _updateLocker = new SemaphoreSlim(1, 1);

		public MusicService(ISongRepository songRepository, IUploadRepository uploadRepository, ISpinnerService spinnerService, IJSRuntime jsRuntime)
        {
            _songRepository = songRepository;
            _uploadRepository = uploadRepository;
            _spinnerService = spinnerService;
            _jsRuntime = jsRuntime;
        }

        public async Task UpdateSongs()
        {
	        await _songRepository.UpdateSongs();
            await PopulateAlbums();
        }

        public async Task<List<Song>> GetAllSongs()
        {
	        List<Song> songs = null;
	        try
	        {
		        _spinnerService.Show();
		        songs = await _songRepository.GetAllSongs();
	        }
	        catch
	        {
				// TODO: error handling
				throw;
			}
	        finally
	        {
		        _spinnerService.Hide();
	        }
		    return songs;
        }

        public async Task<List<Album>> GetAlbums()
        {
            if (_albums == null)
                await PopulateAlbums();
            return _albums;
        }

        public async Task<string> GetSongUrl(int fileID)
        {
			// Why two different URL's? Because the cache API in the browser can't negotiate against the byte-range (HTTP 206) results
			// that the "normal" API endpoint uses for the HTML <audio> element to stream in chunks. The /GetWholeSong endpoint fetches
			// the entire thing and returns a 200, which the cache API is happy to tuck away in storage.
			var cachedUrl = _songRepository.GetCachedSongUrl(fileID);
			var isCached = await _songRepository.IsSongCached(cachedUrl);
            return isCached ? cachedUrl : _songRepository.GetSongUrl(fileID);
        }

        public async Task<bool> Upload(string fileName, Stream stream)
        {
            var isSuccess = await _uploadRepository.UploadFile(fileName, stream);
            return isSuccess;
        }

        private async Task PopulateAlbums()
        {
	        try
	        {
		        await _updateLocker.WaitAsync();
		        _spinnerService.Show();
		        var allSongs = await _songRepository.GetAllSongs();
		        var stopwatch = new Stopwatch();
		        stopwatch.Start();
		        AlbumGroupingType GetGroupingType(Song song)
		        {
			        if (song.AlbumArtist != null) return AlbumGroupingType.AlbumArtist;
			        if (song.Artist != null) return AlbumGroupingType.Artist;
			        return AlbumGroupingType.VariousArtists;
		        }

		        _albums = allSongs.GroupBy(x => new
			        {
				        AlbumArtist = x.AlbumArtist ?? x.Artist ?? "Various Artists",
				        x.Album,
				        AlbumGroupingType = GetGroupingType(x)
			        })
			        .Select(x => new Album {AlbumArtist = x.Key.AlbumArtist, Title = x.Key.Album, AlbumGroupingType = x.Key.AlbumGroupingType, 
				        Songs = x
				        .OrderBy(s => s.Disc)
				        .ThenBy(s => s.Track).ToList(), 
				        FirstSong = x.FirstOrDefault(s => s.PictureMimeType != null)})
			        .Where(x => x.Title != null & x.Title != string.Empty)
			        .OrderBy(x => x.Title)
			        .ToList();
		        stopwatch.Stop();
		        Console.WriteLine($"PopulateAlbums: {stopwatch.ElapsedMilliseconds}ms");
	        }
	        catch
	        {
		        // TODO: error handler
		        throw;
	        }
	        finally
	        {
		        _updateLocker.Release();
		        _spinnerService.Hide();
	        }
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        await _songRepository.IncrementPlayCount(fileID);
        }

        public async Task<List<string>> GetAllArtists()
        {
	        if (_artists != null)
		        return _artists;
	        try
	        {
		        var songs = await _songRepository.GetAllSongs();
		        _spinnerService.Show();
		        var stopwatch = new Stopwatch();
		        stopwatch.Start();
		        var artists = songs
			        .Select(x => x.AlbumArtist ?? x.Artist)
			        .Distinct()
			        .OrderBy(x => x).ToList();
		        if (string.IsNullOrEmpty(artists[0]))
			        artists.RemoveAt(0); // remove anything that came back with no data in Artist or AlbumArtist
		        _artists = artists;
		        stopwatch.Stop();
		        Console.WriteLine($"GetAllArtists: {stopwatch.ElapsedMilliseconds}ms");
	        }
	        catch
	        {
		        // TODO: error handler
		        throw;
	        }
	        finally
	        {
		        _spinnerService.Hide();
	        }

	        return _artists;
        }

        public async Task<Tuple<List<Album>, List<Song>>> GetArtistDetail(string artist)
        {
	        if (_albums == null)
		        await PopulateAlbums();
	        var albums = _albums.Where(x => x.AlbumArtist == artist).ToList();
	        var allSongs = await _songRepository.GetAllSongs();
	        var songs = allSongs.Where(x => x.AlbumArtist == artist || x.Artist == artist).OrderBy(x => x.Title).ToList();
	        return Tuple.Create(albums, songs);
        }

        public async Task DownloadSongList(IEnumerable<Song> songs)
        {
	        foreach (var song in songs)
			{
				var cachedUrl = _songRepository.GetCachedSongUrl(song.FileID);
				await _songRepository.CacheSong(cachedUrl);
			}
        }

        public async Task<bool> IsAllSongsCached(IEnumerable<Song> songs)
		{
			var list = songs.ToList();
			if (!list.Any())
				return false;
	        bool isAllSongsCached = true;
	        foreach (var song in list)
	        {
		        var url = _songRepository.GetCachedSongUrl(song.FileID);
		        var isSongCached = await _songRepository.IsSongCached(url);
		        if (!isSongCached)
			        isAllSongsCached = false;
	        }
	        return isAllSongsCached;
		}

        public async Task RemoveSongsFromCache(IEnumerable<Song> songs)
        {
	        foreach (var song in songs)
			{
				var url = _songRepository.GetCachedSongUrl(song.FileID);
				await _songRepository.DeleteFromCache(url);
			}
        }

		public async Task DeleteCache()
        {
	        await _songRepository.DeleteCache();
        }

		public async Task ScrollReset()
		{
			await _jsRuntime.InvokeVoidAsync("ScrollReset");
		}
	}
}