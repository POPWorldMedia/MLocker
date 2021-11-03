using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MLocker.Core.Models;
using MLocker.Core.Services;

namespace MLocker.WebApp.Services
{
    public interface IPlayerService
    {
        Song CurrentSong { get; }
        List<Song> Queue { get; }
        int QueueIndex { get; }
        void PlaySong(Song song, List<Song> list, int index);
        void PlayNextSong();
        void PlayPreviousSong();
        void EnqueueSong(Song song);
        event Action OnChange;
        void SkipToSong(int index);
        void PlaySongNext(Song song);
        Task<bool> TogglePlayer();
    }

    public class PlayerService : IPlayerService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IFileNameParsingService _fileNameParsingService;
        private List<Song> _queue = new List<Song>();
        private int _queueIndex;
        private Song _currentSong;

        public PlayerService(IJSRuntime jsRuntime, IFileNameParsingService fileNameParsingService)
        {
	        _jsRuntime = jsRuntime;
	        _fileNameParsingService = fileNameParsingService;
        }

        public Song CurrentSong => _currentSong;

        public List<Song> Queue => _queue;

        public int QueueIndex => _queueIndex;

        private void CallPlayerAndUpdateTitle()
        {
	        _jsRuntime.InvokeAsync<string>("StartPlayer", $"{ApiPaths.GetWholeSong}/{_currentSong.FileID}");
	        var imageUrl = QueryHelpers.AddQueryString(ApiPaths.GetImage, "fileName", _fileNameParsingService.ParseImageFileName(_currentSong));
            _jsRuntime.InvokeAsync<string>("SetTitle", _currentSong, imageUrl);
	        Notify();
        }

        public async Task<bool> TogglePlayer()
        {
	        var isPlaying = await _jsRuntime.InvokeAsync<bool>("TogglePlayer");
	        return isPlaying;
        }

        public void PlaySong(Song song, List<Song> list, int index)
        {
            _currentSong = song;
            _queue = list;
            _queueIndex = index;
            CallPlayerAndUpdateTitle();
        }

        public void SkipToSong(int index)
        {
	        _queueIndex = index;
	        _currentSong = _queue[index];
	        CallPlayerAndUpdateTitle();
        }

        public void PlayNextSong()
        {
	        _queueIndex++;
	        if (_queue == null || _queueIndex >= _queue.Count)
	        {
		        _queue = new List<Song>();
		        _queueIndex = 0;
		        _currentSong = null;
		        Notify();
		        return;
	        }

	        _currentSong = _queue[_queueIndex];
	        CallPlayerAndUpdateTitle();
        }

        public void PlayPreviousSong()
        {
	        if (_queueIndex == 0)
	        {
		        return;
	        }

	        _queueIndex--;
	        _currentSong = _queue[_queueIndex];
	        CallPlayerAndUpdateTitle();
        }

        public void EnqueueSong(Song song)
        {
	        if (_queue == null || _queue.Count == 0)
	        {
		        var dictionary = new List<Song> {song};
		        PlaySong(song, dictionary, 0);
	        }
	        else
	        {
		        _queue.Add(song);
				Notify();
	        }
        }

        public void PlaySongNext(Song song)
        {
	        if (_queue == null || _queue.Count == 0 || _currentSong == null)
	        {
		        var dictionary = new List<Song> { song };
		        PlaySong(song, dictionary, 0);
	        }
	        else
	        {
		        var index = _queue.IndexOf(_currentSong);
		        _queue.Insert(index + 1, song);
		        Notify();
	        }
        }

        public event Action OnChange;
        private void Notify() => OnChange?.Invoke();
    }
}