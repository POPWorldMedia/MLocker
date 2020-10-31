var CACHE_NAME = 'music-image-cache';

window.StartPlayer = (wholepath) => {
	var player = document.getElementById('player');
	player.oncanplaythrough = (e) => { player.play() };
	player.onended = (e) => {
		DotNet.invokeMethodAsync('MLocker.WebApp', 'SongEnded');
	};
	//caches.open(CACHE_NAME).then((cache) => {
	//	cache.add(wholepath);
	//});
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
	var instance = Popper.createPopper(button, menu, {
		placement: 'right-start',
		modifiers: [
			{
				name: 'flip',
				options: {
					fallbackPlacements: ['left-start', 'top']
				}
			}
		]
	});
	menu.setAttribute('data-show', '');
	var hideEvents = ['mouseleave', 'blur', 'click'];
	hideEvents.forEach(event => {
		menu.addEventListener(event, () => {
			if (menu)
				menu.removeAttribute('data-show');
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
		navigator.mediaSession.setActionHandler('play', () => player.play());
		navigator.mediaSession.setActionHandler('pause', () => player.pause());
		navigator.mediaSession.setActionHandler('nexttrack', () => DotNet.invokeMethodAsync('MLocker.WebApp', 'SongNext'));
		navigator.mediaSession.setActionHandler('previoustrack', () => DotNet.invokeMethodAsync('MLocker.WebApp', 'SongPrevious'));
	}
}

if ('serviceWorker' in navigator) {
	window.addEventListener('load', function () {
		navigator.serviceWorker.register('/sw.js').then(function (registration) {
			registration.update();
			console.log('ServiceWorker registration successful with scope: ', registration.scope);
		}, function (err) {
			console.log('ServiceWorker registration failed: ', err);
		});
	});
}