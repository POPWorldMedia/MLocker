﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
    public interface ISongRepository
    {
        Task UpdateSongs();
        Task<List<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        string GetCachedSongUrl(int fileID);
        Task IncrementPlayCount(int fileID);
        Task<string> GetRemoteSongListVersion();
        Task<bool> IsSongCached(string url);
        Task CacheSong(string url);
        Task DeleteCache();
        Task DeleteFromCache(string url);
    }

    public class SongRepository : ISongRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfig _config;
        private readonly ILocalStorageRepository _localStorageRepository;
        private readonly IJSRuntime _jsRuntime;
        private static List<Song> _allSongs;
        private static SemaphoreSlim _updateLocker = new SemaphoreSlim(1, 1);
        public const string SongListVersionKey = "SongListVersionKey";
        public const string SongListKey = "SongList";

        public SongRepository(HttpClient httpClient, IConfig config, ILocalStorageRepository localStorageRepository, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _config = config;
            _localStorageRepository = localStorageRepository;
            _jsRuntime = jsRuntime;
        }

        public async Task UpdateSongs()
        {
	        _allSongs = null;
			await GetAllSongs();
        }

        public async Task<List<Song>> GetAllSongs()
        {
	        await _updateLocker.WaitAsync(); // not crazy about this, but first load this can happen concurrently because of component init timing
	        if (_allSongs != null && _allSongs.Any())
	        {
		        _updateLocker.Release();
		        return _allSongs;
	        }

	        try
	        {
		        var stopwatch = new Stopwatch();
		        stopwatch.Start();
		        var songListVersion = await _localStorageRepository.GetItem(SongListVersionKey);
		        var remoteSongListVersion = await GetRemoteSongListVersion();
		        var isNewVersion = songListVersion != remoteSongListVersion;

                if (isNewVersion)
                {
                    await FetchSongList();
                }
                else
                {
                    var rawObject = await _localStorageRepository.GetItem(SongListKey);
                    if (rawObject == null)
                        await FetchSongList();
                    else
                        _allSongs = JsonSerializer.Deserialize<List<Song>>(rawObject);
                }

		        stopwatch.Stop();
		        Console.WriteLine($"Song fetch: {stopwatch.ElapsedMilliseconds}ms");
	        }
	        catch
	        {
		        throw; // TODO: error handling
	        }
	        finally
	        {
		        _updateLocker.Release();
	        }

	        return _allSongs;
        }

        private async Task FetchSongList()
        {
            var apiKey = await _config.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
            var response = await _httpClient.GetStringAsync(ApiPaths.GetAllSongs);
            var payload = JsonSerializer.Deserialize<SongListPayload>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var serializedList = JsonSerializer.Serialize(payload.Songs);
            await _localStorageRepository.SetItem(SongListKey, serializedList);
            await _localStorageRepository.SetItem(SongListVersionKey, payload.Version);
            _allSongs = payload.Songs.ToList();
        }

        public string GetSongUrl(int fileID)
        {
            var url = $"{ApiPaths.GetSong}/{fileID}";
            return url;
		}

        public string GetCachedSongUrl(int fileID)
        {
	        var url = $"{ApiPaths.GetWholeSong}/{fileID}";
	        return url;
        }

		public async Task IncrementPlayCount(int fileID)
        {
	        var song = _allSongs.SingleOrDefault(x => x.FileID == fileID);
	        if (song != null)
		        song.PlayCount++;
	        var apiKey = await _config.GetApiKey();
	        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
	        var data = new {FileID = fileID};
	        await _httpClient.PostAsJsonAsync(ApiPaths.IncrementPlayCount, data);
        }

        public async Task<string> GetRemoteSongListVersion()
		{
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			_httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
			var songListVersion = await _httpClient.GetStringAsync(ApiPaths.GetSongListVersion);
	        return songListVersion;
        }

        public async Task<bool> IsSongCached(string url)
        {
	        var isCached = await _jsRuntime.InvokeAsync<bool>("IsUrlCached", url);
	        return isCached;
        }

        public async Task CacheSong(string url)
        {
	        await _jsRuntime.InvokeVoidAsync("AddToCache", url);
		}

        public async Task DeleteFromCache(string url)
        {
	        await _jsRuntime.InvokeVoidAsync("RemoveFromCache", url);
        }

		public async Task DeleteCache()
        {
	        await _jsRuntime.InvokeVoidAsync("ClearCache");
        }
	}
}