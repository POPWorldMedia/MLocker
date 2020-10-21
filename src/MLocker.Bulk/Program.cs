using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.Bulk
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Enter the API base URL (no trailing slash):");
			var apiBaseUrl = Console.ReadLine();
			Console.WriteLine("What's the API key?");
			var apiKey = Console.ReadLine();
			Console.WriteLine("What's the local folder path to upload?");
			var baseFolder = Console.ReadLine();

			var fileList = new List<string>();
			fileList.AddRange(Directory.EnumerateFiles(baseFolder, "*.mp3", SearchOption.AllDirectories));
			fileList.AddRange(Directory.EnumerateFiles(baseFolder, "*.m4a", SearchOption.AllDirectories));
			Console.WriteLine($"Total files found: {fileList.Count}");

			var errorList = new List<string>();
			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			client.BaseAddress = new Uri(apiBaseUrl);
			foreach (var file in fileList)
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var fileName = Path.GetFileName(file);
				using var content = new MultipartFormDataContent();
				await using var stream = File.OpenRead(file);
				using var streamContent = new StreamContent(stream, Convert.ToInt32(stream.Length));
				content.Add(streamContent, "file", fileName);
				content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

				using var response = await client.PostAsync(ApiPaths.Upload, content);
				streamContent.Dispose();
				content.Dispose();
				var result = response.IsSuccessStatusCode;
				response.Dispose();
				if (!result)
					errorList.Add(file);
				stopwatch.Stop();
				Console.WriteLine($"{file} {stopwatch.ElapsedMilliseconds}ms");
			}

			Console.WriteLine($"Uploads complete, {fileList.Count} files processed, {errorList.Count} errors:");
			foreach (var item in errorList)
				Console.WriteLine(@"\t{item}");

			Console.ReadLine();
		}
	}
}
