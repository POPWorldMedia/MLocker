using System.Collections.Generic;

namespace MLocker.Core.Models
{
	public class PlaylistPayload
	{
		public string Version { get; set; }
		public IEnumerable<PlaylistDefinition> PlaylistDefinitions { get; set; }
	}
}