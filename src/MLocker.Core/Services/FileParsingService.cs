using MLocker.Core.Models;

namespace MLocker.Core.Services
{
	public class FileParsingService
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
				Year = file.Tag.Year,
				Track = file.Tag.Track,
				TrackCount = file.Tag.TrackCount,
				Disc = file.Tag.Disc,
				DiscCount = file.Tag.DiscCount,
				Length = file.Properties.Duration,
				Picture = file.Tag.Pictures?[0]?.Data?.Data,
				PictureMimeType = file.Tag.Pictures?[0]?.MimeType,
				FileName = fileName
			};
			return fileData;
		}
	}
}