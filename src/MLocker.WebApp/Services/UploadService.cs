using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IUploadService
    {
        Task Upload(SongData songData, byte[] fileData);
    }

    public class UploadService : IUploadService
    {
        private readonly IUploadRepository _uploadRepository;

        public UploadService(IUploadRepository uploadRepository)
        {
            _uploadRepository = uploadRepository;
        }

        public async Task Upload(SongData songData, byte[] fileData)
        {
            var container = new UploadContainer
            {
                File = fileData,
                Picture = songData.Picture,
                Song = songData
            };
            await _uploadRepository.UploadFile(container);
        }
    }
}