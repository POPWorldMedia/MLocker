using System.Threading.Tasks;
using MLocker.Api.Repositories;
using MLocker.Core.Models;

namespace MLocker.Api.Services
{
    public interface ISongService
    {
        Task PersistSong(SongData songData, byte[] bytes);
        string ParseStorageFileName(SongData songData);
    }

    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IFileRepository _fileRepository;

        public SongService(ISongRepository songRepository, IFileRepository fileRepository)
        {
            _songRepository = songRepository;
            _fileRepository = fileRepository;
        }

        public async Task PersistSong(SongData songData, byte[] bytes)
        {
            await _songRepository.SaveSong(songData);
            var fileName = ParseStorageFileName(songData);
            await _fileRepository.SaveFile(fileName, bytes);
            // TODO: persist image logic, should be one per album, so that's not straight forward yet
        }

        public string ParseStorageFileName(SongData songData)
        {
            var name = $"files/{songData.AlbumArtist}/{songData.Album}/{songData.Disc}-{songData.Track}- {songData.Title}";
            return name;
        }
    }
}