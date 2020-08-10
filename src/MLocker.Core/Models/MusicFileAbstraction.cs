using System.IO;

namespace MLocker.Core.Services.Models
{
	public class MusicFileAbstraction : TagLib.File.IFileAbstraction
	{
		private readonly MemoryStream _stream;
		
		public MusicFileAbstraction(string name, byte[] bytes)
		{
			Name = name;
			_stream = new MemoryStream(bytes);
		}

		public void CloseStream(Stream stream)
		{
			_stream.Close();
		}

		public string Name { get; }
		public Stream ReadStream => _stream;
		public Stream WriteStream => _stream;
	}
}