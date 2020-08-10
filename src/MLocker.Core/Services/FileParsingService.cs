using MLocker.Core.Services.Models;

namespace MLocker.Core.Services
{
	public class FileParsingService
	{
		public FileData ReadFileData(string name, byte[] fileBytes)
		{
			var container = new MusicFileAbstraction(name, fileBytes);
			var file = TagLib.File.Create(container);
			var fileData = new FileData
			{
				Title = file.Tag.Title,
				Artist = file.Tag.FirstPerformer,
				AlbumArtist = file.Tag.FirstAlbumArtist,
				Album = file.Tag.Album,
				Composer = file.Tag.FirstComposer,
				Genre = file.Tag.FirstGenre,
				Year = file.Tag.Year,
				Track = file.Tag.Track,
				TrackCount = file.Tag.TrackCount,
				Disc = file.Tag.Disc,
				DiscCount = file.Tag.DiscCount,
				Length = file.Properties.Duration,
				Picture = file.Tag.Pictures?[0]?.Data?.Data,
				PictureMimeType = file.Tag.Pictures?[0]?.MimeType
			};
			return fileData;
		}
	}
}