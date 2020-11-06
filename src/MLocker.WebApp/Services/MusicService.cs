using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IMusicService
    {
        Task<List<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        Task<bool> Upload(string fileName, Stream stream);
        Task<List<Album>> GetAlbums();
        Task IncrementPlayCount(int fileID);
        Task<List<string>> GetAllArtists();
        Task<Tuple<List<Album>, List<Song>>> GetArtistDetail(string artist);
        Task UpdateSongs();
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private readonly IUploadRepository _uploadRepository;
        private readonly ISpinnerService _spinnerService;
        private static List<Album> _albums;
        private static List<string> _artists;

        public MusicService(ISongRepository songRepository, IUploadRepository uploadRepository, ISpinnerService spinnerService)
        {
            _songRepository = songRepository;
            _uploadRepository = uploadRepository;
            _spinnerService = spinnerService;
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

        public string GetSongUrl(int fileID)
        {
            return _songRepository.GetSongUrl(fileID);
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
		        _spinnerService.Show();
		        var stopwatch = new Stopwatch();
		        stopwatch.Start();
		        var songs = await _songRepository.GetAllSongs();
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
    }
}