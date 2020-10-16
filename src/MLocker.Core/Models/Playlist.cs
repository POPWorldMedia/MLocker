using System.Collections.Generic;

namespace MLocker.Core.Models
{
	public class Playlist
	{
		public int PlaylistID { get; set; }
		public string Title { get; set; }
		public List<Song> Songs { get; set; }
	}
}