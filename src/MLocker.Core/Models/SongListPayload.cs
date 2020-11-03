using System.Collections.Generic;

namespace MLocker.Core.Models
{
	public class SongListPayload
	{
		public string Version { get; set; }
		public IEnumerable<Song> Songs { get; set; }
	}
}