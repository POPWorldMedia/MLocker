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
        void EnqueueSong(Song song);
        void PlaySong(Song song);
        event Action OnChange;
    }

    public class PlayerService : IPlayerService
    {
        private readonly IJSRuntime _jsRuntime;
        private List<Song> _queue;
        private Song _currentSong;
        private string _songUrl;

        public PlayerService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _queue = new List<Song>();
        }

        public Song CurrentSong
        {
            get => _currentSong;
        }

        public List<Song> Queue
        {
            get => _queue;
        }

        public void PlaySong(Song song)
        {
            _currentSong = song;
            Notify();
            _jsRuntime.InvokeAsync<string>("StartPlayer");
        }

        public void EnqueueSong(Song song)
        {

        }

        public event Action OnChange;
        private void Notify() => OnChange?.Invoke();
    }
}