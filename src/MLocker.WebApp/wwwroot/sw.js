var CACHE_NAME = 'music-image-cache';

self.addEventListener('install', event => event.waitUntil(onInstall(event)));

async function onInstall(event) {
	self.skipWaiting();
}

self.addEventListener('fetch', event => {
	var cacheControl = event.request.headers.get('cache-control');
	if (cacheControl !== 'no-cache')
		if (event.request.url.includes('GetImage')
			|| event.request.url.includes('GetAllSongs')
			|| event.request.url.includes('GetAllPlaylistDefinitions')
			|| event.request.url.includes('GetWholeSong')) {
			event.respondWith(
				caches.open(CACHE_NAME).then(function (cache) {
					return cache.match(event.request).then(function (response) {
						return response || fetch(event.request).then(function (response) {
							cache.put(event.request, response.clone());
							return response;
						});
					});
				})
			);
		}
});
