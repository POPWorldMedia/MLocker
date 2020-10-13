using System.Collections.Generic;
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
        Task Upload(string fileName, Stream stream);
        Task<List<Album>> GetAlbums();
        Task<List<Song>> GetAlbum(string albumArtist, string title, AlbumGroupingType albumGroupingType);
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private readonly IUploadRepository _uploadRepository;
        private static List<Song> _songs;
        private static List<Album> _albums;

        public MusicService(ISongRepository songRepository, IUploadRepository uploadRepository)
        {
            _songRepository = songRepository;
            _uploadRepository = uploadRepository;
        }

        public async Task UpdateSongs()
        {
            var songs = await _songRepository.GetAllSongs();
            _songs = songs.OrderBy(x => x.Title).ToList();
            await PopulateAlbums();
        }

        public async Task<List<Song>> GetAllSongs()
        {
            if (_songs == null)
                await UpdateSongs();
            return _songs;
        }

        public async Task<List<Album>> GetAlbums()
        {
            if (_songs == null)
                await UpdateSongs();
            return _albums;
        }

        public string GetSongUrl(int fileID)
        {
            return _songRepository.GetSongUrl(fileID);
        }

        public async Task Upload(string fileName, Stream stream)
        {
            await _uploadRepository.UploadFile(fileName, stream);
            await UpdateSongs();
        }

        private async Task PopulateAlbums()
        {
            AlbumGroupingType GetGroupingType(Song song)
            {
                if (song.AlbumArtist != null) return AlbumGroupingType.AlbumArtist;
                if (song.Artist != null) return AlbumGroupingType.Artist;
                return AlbumGroupingType.VariousArtists;
            }

            _albums = _songs.GroupBy(x => new { AlbumArtist = x.AlbumArtist ?? x.Artist ?? "Various Artists",
                    x.Album,
                    AlbumGroupingType = GetGroupingType(x)
            })
                .Select(x => new Album {AlbumArtist = x.Key.AlbumArtist, Title = x.Key.Album, AlbumGroupingType = x.Key.AlbumGroupingType})
                .OrderBy(x => x.Title)
                .ToList();
            // this is janky and probably slow, but I couldn't figure out how to do it in the linq query above
            foreach (var album in _albums)
            {
                var songs = await GetAlbum(album.AlbumArtist, album.Title, album.AlbumGroupingType);
                if (songs.Any(x => x.PictureMimeType != null))
                {
                    album.FirstSong = songs.First();
                }
            }
        }

        public async Task<List<Song>> GetAlbum(string albumArtist, string title, AlbumGroupingType albumGroupingType)
        {
            if (_songs == null)
                await UpdateSongs();
            var unsorted = albumGroupingType switch
            {
                AlbumGroupingType.AlbumArtist => _songs.Where(x => x.Album == title && x.AlbumArtist == albumArtist),
                AlbumGroupingType.Artist => _songs.Where(x => x.Album == title && x.Artist == albumArtist),
                _ => _songs.Where(x => x.Album == title && x.AlbumArtist == null && x.Artist == null)
            };
            var songs = unsorted
                .OrderBy(x => x.Disc)
                .ThenBy(x => x.Track)
                .ToList();
            return songs;
        }
    }
}