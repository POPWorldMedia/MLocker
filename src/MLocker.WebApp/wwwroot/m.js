var CACHE_NAME = 'music-image-cache';

window.StartPlayer = (wholepath) => {
	var player = document.getElementById('player');
	var songRange = document.getElementById('songRange');
	var duration = document.getElementById('duration');
	var currentTime = document.getElementById('currentTime');
	player.oncanplaythrough = (e) => {
		songRange.min = 0;
		songRange.max = player.duration;
		duration.innerHTML = FormatSeconds(player.duration);
		player.play();
	};
	player.onended = (e) => {
		songRange.disabled = true;
		DotNet.invokeMethodAsync('MLocker.WebApp', 'SongEnded');
	};
	player.ontimeupdate = () => {
		songRange.value = player.currentTime;
		songRange.disabled = false;
		currentTime.innerHTML = FormatSeconds(player.currentTime);
	};
	songRange.oninput = (e) => {
		player.currentTime = e.target.value;
	}
}

window.TogglePlayer = () => {
	var player = document.getElementById('player');
	if (player.paused) {
		player.play();
		return true;
	} else {
		player.pause();
		return false;
	}
}

window.GetPlayerStatus = () => {
	var player = document.getElementById('player');
	return player.paused;
}

window.FormatSeconds = (s) => {
	var minutes = Math.floor(s / 60);
	var seconds = s % 60;
	return minutes.toString() + ":" + Math.round(seconds).toString().padStart(2, "0");
}

window.OpenApiModal = () => {
	var myModal = new bootstrap.Modal(document.getElementById('apiModal'), { keyboard: false, backdrop: 'static' });
	myModal.show();
}

window.Reload = () => {
	location.reload();
}

window.OpenSongContext = (contextMenuId) => {
	var menu = document.querySelector('#songContextMenu');
	var button = document.querySelector('#' + contextMenuId);
	var songList = button.closest(".songList");
	var instance = Popper.createPopper(button, menu, {
		placement: 'right-start',
		modifiers: [
			{
				name: 'offset',
				options: {
					offset: [0, 0]
				}
			},
			{
				name: 'flip',
				options: {
					boundary: songList
				}
			},
			{
				name: 'preventOverflow',
				options: {
					boundary: songList
				}
			}
		]
	});
	menu.setAttribute('data-bs-show', '');
	var hideEvents = ['mouseleave', 'blur', 'click'];
	hideEvents.forEach(event => {
		menu.addEventListener(event, () => {
			if (menu)
				menu.removeAttribute('data-bs-show');
			if (instance)
				instance.destroy();
			instance = null;
		});
	});
}

window.OpenAddToPlaylistModal = () => {
	var myModal = new bootstrap.Modal(document.getElementById('addToPlaylistModal'));
	myModal.show();
}

window.SetTitle = (song, imageUrl) => {
	document.title = song.title + ' - ' + song.artist + ' - MLocker';
	var player = document.getElementById('player');
	if ('mediaSession' in navigator) {
		navigator.mediaSession.metadata = new MediaMetadata({
			title: song.title,
			artist: song.artist,
			album: song.album,
			artwork: [
				{ src: imageUrl, sizes: '300x300', type: song.mediaMimeType }
			]
		});
		navigator.mediaSession.setActionHandler('play', () => {
			player.play();
			DotNet.invokeMethodAsync('MLocker.WebApp', 'IsPlaying', true);
		});
		navigator.mediaSession.setActionHandler('pause', () => {
			player.pause();
			DotNet.invokeMethodAsync('MLocker.WebApp', 'IsPlaying', false);
		});
		navigator.mediaSession.setActionHandler('nexttrack', () => DotNet.invokeMethodAsync('MLocker.WebApp', 'SongNext'));
		navigator.mediaSession.setActionHandler('previoustrack', () => DotNet.invokeMethodAsync('MLocker.WebApp', 'SongPrevious'));
	}
}

window.SetStorageItem = (key, value) => {
	localStorage.setItem(key, value);
}

window.GetStorageItem = (key) => {
	return localStorage.getItem(key);
}

window.AddToCache = (url) => {
	return fetch(url)
		.catch(error => console.log('Song not cached: ' + error));
}

window.RemoveFromCache = (url) => {
	return caches.open(CACHE_NAME)
		.then((cache) => {
			return cache.delete(url);
		})
		.catch(error => console.log('Can\'t remove from cache: ' + error));
}

window.IsUrlCached = (url) => {
	return caches.open(CACHE_NAME)
		.then((cache) => {
				return cache.match(url)
					.then((response) => {
						if (typeof response !== 'undefined')
							return true;
						else
							return false;
					});
			}
		)
		.catch(error => console.error(error));
}

window.ClearCache = () => {
	return caches.delete(CACHE_NAME)
		.then(() => {
			console.log('Cache deleted');
		});
}

window.ScrollReset = () => {
	var topper = document.getElementById('topper');
	topper.scrollIntoView();
}

if ('serviceWorker' in navigator) {
	window.addEventListener('load', function () {
		navigator.serviceWorker.register('sw.js').then(function (registration) {
			registration.update();
			console.log('ServiceWorker registration successful with scope: ', registration.scope);
		}, function (err) {
			console.log('ServiceWorker registration failed: ', err);
		});
	});
}
