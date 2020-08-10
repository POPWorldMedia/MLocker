using System;
using MLocker.Core.Files;
using File = System.IO.File;

namespace MLocker.ScratchConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var path = @"D:\Music\Amazon MP3\Absofacto\Thousand Peaces\01-01- Dissolve.mp3";
			var bytes = File.ReadAllBytes(path);
			var ingester = new Ingester();
			ingester.ReadFileData("Dissolve.mp3", bytes);
		}
	}
}
