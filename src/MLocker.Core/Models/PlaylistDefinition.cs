using System.Collections.Generic;

namespace MLocker.Core.Models
{
	public class PlaylistDefinition
	{
		public int PlaylistID { get; set; }
		public string Title { get; set; }
		public List<int> SongIDs { get; set; }
	}
}