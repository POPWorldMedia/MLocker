using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        Task<List<Song>> GetAlbum(Album album);
        Task UpdateSongs();
        Task IncrementPlayCount(int fileID);
        Task<List<string>> GetAllArtists();
        Task<Tuple<List<Album>, List<Song>>> GetArtistDetail(string artist);
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private readonly IUploadRepository _uploadRepository;
        private static List<Album> _albums;
        private static List<string> _artists;

        public MusicService(ISongRepository songRepository, IUploadRepository uploadRepository)
        {
            _songRepository = songRepository;
            _uploadRepository = uploadRepository;
        }

        public async Task UpdateSongs()
        {
	        await _songRepository.UpdateSongs();
            await PopulateAlbums();
        }

        public async Task<List<Song>> GetAllSongs()
        {
	        var songs = await _songRepository.GetAllSongs();
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
            return await _uploadRepository.UploadFile(fileName, stream);
        }

        private async Task PopulateAlbums()
        {
            AlbumGroupingType GetGroupingType(Song song)
            {
                if (song.AlbumArtist != null) return AlbumGroupingType.AlbumArtist;
                if (song.Artist != null) return AlbumGroupingType.Artist;
                return AlbumGroupingType.VariousArtists;
            }

            var allSongs = await _songRepository.GetAllSongs();
            _albums = allSongs.GroupBy(x => new { AlbumArtist = x.AlbumArtist ?? x.Artist ?? "Various Artists",
                    x.Album,
                    AlbumGroupingType = GetGroupingType(x)
            })
                .Select(x => new Album {AlbumArtist = x.Key.AlbumArtist, Title = x.Key.Album, AlbumGroupingType = x.Key.AlbumGroupingType})
                .Where(x => x.Title != null & x.Title != string.Empty)
                .OrderBy(x => x.Title)
                .ToList();
            // this is janky and probably slow, but I couldn't figure out how to do it in the linq query above
            foreach (var album in _albums)
            {
                var songs = await GetAlbum(album);
                if (songs.Any(x => x.PictureMimeType != null))
                {
                    album.FirstSong = songs.First(x => x.PictureMimeType != null);
                }
            }
        }

        public async Task<List<Song>> GetAlbum(Album album)
        {
            if (_albums == null)
                await UpdateSongs();
            var allSongs = await _songRepository.GetAllSongs();
            var unsorted = album.AlbumGroupingType switch
            {
                AlbumGroupingType.AlbumArtist => allSongs.Where(x => x.Album == album.Title && x.AlbumArtist == album.AlbumArtist),
                AlbumGroupingType.Artist => allSongs.Where(x => x.Album == album.Title && x.Artist == album.AlbumArtist),
                _ => allSongs.Where(x => x.Album == album.Title && x.AlbumArtist == null && x.Artist == null)
            };
            var songs = unsorted
                .OrderBy(x => x.Disc)
                .ThenBy(x => x.Track)
                .ToList();
            return songs;
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        await _songRepository.IncrementPlayCount(fileID);
        }

        public async Task<List<string>> GetAllArtists()
        {
	        if (_artists != null)
		        return _artists;
	        var songs = await _songRepository.GetAllSongs();
	        var artists = songs.Where(x => x.AlbumArtist != null)
		        .Select(x => x.AlbumArtist).ToList();
	        artists.AddRange(songs.Where(x => x.AlbumArtist == null && x.Artist != null).Select(x => x.Artist));
	        artists.Sort();
	        _artists = artists.Distinct().ToList();
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