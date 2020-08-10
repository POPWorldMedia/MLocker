using System;

namespace MLocker.Core.Services.Models
{
	public class FileData
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string AlbumArtist { get; set; }
		public string Album { get; set; }
		public string Composer { get; set; }
		public string Genre { get; set; }
		public uint? Year { get; set; }
		public uint? Track { get; set; }
		public uint? TrackCount { get; set; }
		public uint? Disc { get; set; }
		public uint? DiscCount { get; set; }
		public TimeSpan Length { get; set; }
		public int PlayCount { get; set; }
		public byte[] Picture { get; set; }
		public string PictureMimeType { get; set; }
	}
}