using MLocker.Core.Models;

namespace MLocker.Core.Services
{
    public interface IFileParsingService
    {
        SongData ReadFileData(string fileName, byte[] fileBytes);
    }

    public class FileParsingService : IFileParsingService
    {
		public SongData ReadFileData(string fileName, byte[] fileBytes)
		{
			var container = new MusicFileAbstraction(fileName, fileBytes);
			var file = TagLib.File.Create(container);
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
				Picture = file.Tag.Pictures?[0]?.Data?.Data,
				PictureMimeType = file.Tag.Pictures?[0]?.MimeType,
				FileName = fileName
			};
			return fileData;
		}
	}
}