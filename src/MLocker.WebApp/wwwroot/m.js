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

window.SetTitle = (title) => {
	document.title = title;
}
