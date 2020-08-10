
using MLocker.Core.Models;

namespace MLocker.Core.Files
{
	public class Ingester
	{
		public void ReadFileData(string name, byte[] fileBytes)
		{
			var container = new MusicFileAbstraction(name, fileBytes);
			var file = TagLib.File.Create(container);
			var title = file.Tag.Title;
		}
	}
}