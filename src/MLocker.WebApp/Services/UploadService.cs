using System.IO;
using System.Threading.Tasks;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
    public interface IUploadService
    {
        Task Upload(string fileName, Stream stream);
    }

    public class UploadService : IUploadService
    {
        private readonly IUploadRepository _uploadRepository;

        public UploadService(IUploadRepository uploadRepository)
        {
            _uploadRepository = uploadRepository;
        }

        public async Task Upload(string fileName, Stream stream)
        {
            await _uploadRepository.UploadFile(fileName, stream);
        }
    }
}