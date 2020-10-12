using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IMusicService
    {
        Task<IEnumerable<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        Task Upload(string fileName, Stream stream);
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private readonly IUploadRepository _uploadRepository;
        private static IEnumerable<Song> _songs;

        public MusicService(ISongRepository songRepository, IUploadRepository uploadRepository)
        {
            _songRepository = songRepository;
            _uploadRepository = uploadRepository;
        }

        public async Task UpdateSongs()
        {
            _songs = await _songRepository.GetAllSongs();
        }

        public async Task<IEnumerable<Song>> GetAllSongs()
        {
            if (_songs == null)
                await UpdateSongs();
            return _songs;
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
    }
}