using System.IO;
using MLocker.Core.Models;

namespace MLocker.Core.Services
{
    public interface IFileParsingService
    {
        SongData ReadFileData(string fileName, byte[] fileBytes);
        string ParseImageFileName(Song songData);
    }

    public class FileParsingService : IFileParsingService
    {
		public SongData ReadFileData(string fileName, byte[] fileBytes)
		{
			var container = new MusicFileAbstraction(fileName, fileBytes);
			var file = TagLib.File.Create(container);
            byte[] pictureData = file.Tag.Pictures.Length > 0 ? file.Tag.Pictures?[0]?.Data?.Data : null;
            string pictureMimeTime = (file.Tag.Pictures.Length > 0 && pictureData?.Length > 0) ? file.Tag.Pictures?[0]?.MimeType : null;
            var fileType = Path.GetExtension(fileName);
			var fileData = new SongData
			{
				Title = file.Tag.Title ?? fileName,
				Artist = file.Tag.FirstPerformer,
				AlbumArtist = file.Tag.FirstAlbumArtist,
				Album = file.Tag.Album,
				Composer = file.Tag.FirstComposer,
				Genre = file.Tag.FirstGenre,
				Year = (int?)file.Tag.Year,
				Track = (int?)file.Tag.Track,
				TrackCount = (int?)file.Tag.TrackCount,
				Disc = (int?)file.Tag.Disc,
				DiscCount = (int?)file.Tag.DiscCount,
				Length = file.Properties.Duration,
				Picture = pictureData,
				PictureMimeType = pictureMimeTime,
				FileName = fileName,
				FileType = fileType
			};
			return fileData;
		}

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