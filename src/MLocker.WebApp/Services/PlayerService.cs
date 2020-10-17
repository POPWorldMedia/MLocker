using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using MLocker.Core.Models;

namespace MLocker.WebApp.Services
{
    public interface IPlayerService
    {
        Song CurrentSong { get; }
        Dictionary<int, Song> Queue { get; }
        void PlaySong(Song song, Dictionary<int, Song> dictionary, int index);
        void PlayNextSong();
        void PlayPreviousSong();
        void EnqueueSong(Song song);
        event Action OnChange;
        Dictionary<int, Song> GetIndexedList(List<Song> songs);
        void SkipToSong(int index);
    }

    public class PlayerService : IPlayerService
    {
        private readonly IJSRuntime _jsRuntime;
        private Dictionary<int, Song> _queue = new Dictionary<int, Song>();
        private int _queueIndex;
        private Song _currentSong;

        public PlayerService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public Song CurrentSong
        {
            get => _currentSong;
        }

        public Dictionary<int, Song> Queue
        {
            get => _queue;
        }

        public void PlaySong(Song song, Dictionary<int, Song> dictionary, int index)
        {
            _currentSong = song;
            _queue = dictionary;
            _queueIndex = index;
            Notify();
            _jsRuntime.InvokeAsync<string>("StartPlayer");
        }

        public void SkipToSong(int index)
        {
	        _queueIndex = index;
	        _currentSong = _queue[index];
	        Notify();
	        _jsRuntime.InvokeAsync<string>("StartPlayer");
        }

        public void PlayNextSong()
        {
	        _queueIndex++;
	        if (_queue == null || _queueIndex >= _queue.Count)
	        {
		        _queue = null;
		        _queueIndex = 0;
		        _currentSong = null;
		        Notify();
		        return;
	        }

	        _currentSong = _queue[_queueIndex];
	        Notify();
	        _jsRuntime.InvokeAsync<string>("StartPlayer");
        }

        public void PlayPreviousSong()
        {
	        if (_queueIndex == 0)
	        {
		        return;
	        }

	        _queueIndex--;
	        _currentSong = _queue[_queueIndex];
	        Notify();
	        _jsRuntime.InvokeAsync<string>("StartPlayer");
        }

        public void EnqueueSong(Song song)
        {

        }

        public event Action OnChange;
        private void Notify() => OnChange?.Invoke();

        public Dictionary<int, Song> GetIndexedList(List<Song> songs)
        {
	        var x = 0;
	        var dictionary = new Dictionary<int, Song>();
	        foreach (var item in songs)
	        {
		        dictionary.Add(x, item);
		        x++;
	        }
	        return dictionary;
        }
    }
}