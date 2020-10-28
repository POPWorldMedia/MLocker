using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using MLocker.Core.Models;

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
    }

    public class PlayerService : IPlayerService
    {
        private readonly IJSRuntime _jsRuntime;
        private List<Song> _queue = new List<Song>();
        private int _queueIndex;
        private Song _currentSong;

        public PlayerService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public Song CurrentSong => _currentSong;

        public List<Song> Queue => _queue;

        public int QueueIndex => _queueIndex;

        private void CallPlayerAndUpdateTitle()
        {
	        Notify();
	        _jsRuntime.InvokeAsync<string>("StartPlayer");
	        var title = $"{_currentSong.Title ?? _currentSong.FileName} - {_currentSong.Artist ?? _currentSong.AlbumArtist} - MLocker";
	        _jsRuntime.InvokeAsync<string>("SetTitle", title);
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
	        if (_queue == null || _queue.Count == 0)
	        {
		        var dictionary = new List<Song> { song };
		        PlaySong(song, dictionary, 0);
	        }
	        else
	        {
		        _queue.Insert(1, song);
		        Notify();
	        }
        }

        public event Action OnChange;
        private void Notify() => OnChange?.Invoke();
    }
}