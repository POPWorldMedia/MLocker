using MLocker.Core.Models;

namespace MLocker.Core.Services
{
    public interface IFileNameParsingService
    {
        string ParseImageFileName(Song songData);
    }

    public class FileNameParsingService : IFileNameParsingService
    {
        public string ParseImageFileName(Song songData)
        {
            var ext = songData.PictureMimeType switch
            {
                "image/jpeg" => "jpg",
                "image/jpg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                _ => string.Empty
            };
            var name = $"images/{songData.AlbumArtist ?? songData.Artist ?? "Various Artists"}/{songData.Album ?? "No Album"}.{ext}";
            return name;
        }
    }
}
