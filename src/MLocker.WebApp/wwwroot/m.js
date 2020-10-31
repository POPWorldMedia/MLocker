window.StartPlayer = () => {
	var player = document.getElementById('player');
	player.oncanplaythrough = (e) => { player.play() };
	player.onended = (e) => {
		DotNet.invokeMethodAsync('MLocker.WebApp', 'SongEnded');
	};
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
		placement: 'right-start'
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
