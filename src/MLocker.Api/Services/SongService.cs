using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MLocker.Api.Repositories;
using MLocker.Core.Models;
using MLocker.Core.Services;

namespace MLocker.Api.Services
{
    public interface ISongService
    {
        Task PersistSong(SongData songData, byte[] bytes);
        string ParseStorageFileName(Song songData);
        Task<IEnumerable<Song>> GetAll();
        Task<Tuple<Stream, Song>> GetSong(int songID);
        Task<Tuple<Stream, string>> GetImage(string imageName);
        Task IncrementPlayCount(int fileID);
        Task<string> GetSongListVersion();
    }

    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileParsingService _fileParsingService;
        private readonly IVersionRepository _versionRepository;

        public SongService(ISongRepository songRepository, IFileRepository fileRepository, IFileParsingService fileParsingService, IVersionRepository versionRepository)
        {
            _songRepository = songRepository;
            _fileRepository = fileRepository;
            _fileParsingService = fileParsingService;
            _versionRepository = versionRepository;
        }

        public async Task PersistSong(SongData songData, byte[] bytes)
        {
	        await _songRepository.DeleteSong(songData.Title, songData.Album, songData.Artist);
            await _songRepository.SaveSong(songData);
            var fileName = ParseStorageFileName(songData);
            var contentType = songData.FileType switch
            {
                ".mp3" => "audio/mpeg",
                ".m4a" => "audio/m4a",
                _ => "unknown"
            };
            await _fileRepository.SaveFile(fileName, bytes, contentType);
            if (songData.Picture != null && songData.Picture.Length > 0)
            {
                var imageFileName = _fileParsingService.ParseImageFileName(songData);
                await _fileRepository.SaveFile(imageFileName, songData.Picture, songData.PictureMimeType);
            }

            var version = Guid.NewGuid().ToString();
            await _versionRepository.SaveSongListVersion(version);
        }

        public string ParseStorageFileName(Song songData)
        {
            var name = $"files/{songData.AlbumArtist?.Replace("/","_") ?? songData.Artist?.Replace("/", "_") ?? "Various Artists"}/{songData.Album?.Replace("/", "_") ?? "No Album"}/{songData.Disc?.ToString("D2") ?? "00"}-{songData.Track?.ToString("D2") ?? "00"}- {songData.Title?.Replace("/", "_") ?? songData.FileName}{songData.FileType}";
            return name;
        }

        public async Task<IEnumerable<Song>> GetAll()
        {
            return await _songRepository.GetAll();
        }

        public async Task<Tuple<Stream, Song>> GetSong(int songID)
        {
            var song = await _songRepository.GetSong(songID);
            if (song == null)
                return null;
            var fileName = ParseStorageFileName(song);
            var stream = await _fileRepository.GetFile(fileName);
            return Tuple.Create(stream, song);
        }

        public async Task<Tuple<Stream, string>> GetImage(string imageName)
        {
            // if this were a real production thing where you didn't want people poking around your container, you would parse this
            var stream = await _fileRepository.GetFileWithContentType(imageName);
            return stream;
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        await _songRepository.IncrementPlayCount(fileID);
        }

        public async Task<string> GetSongListVersion()
        {
	        return await _versionRepository.GetSongListVersion();
        }
    }
}