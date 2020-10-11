using System.Collections.Generic;
using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IMusicService
    {
        Task<IEnumerable<Song>> GetAllSongs();
        Task<string> GetSongUrl(int fileID);
    }

    public class MusicService : IMusicService
    {
        private readonly ISongRepository _songRepository;
        private static IEnumerable<Song> _songs;

        public MusicService(ISongRepository songRepository)
        {
            _songRepository = songRepository;
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

        public async Task<string> GetSongUrl(int fileID)
        {
            return await _songRepository.GetSongUrl(fileID);
        }
    }
}