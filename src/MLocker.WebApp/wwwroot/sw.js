var CACHE_NAME = 'music-image-cache';

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

self.importScripts('./service-worker-assets.js');

self.addEventListener('install', event => {
	self.skipWaiting();
	// Fetch and cache all matching items from the assets manifest
	const assetsRequests = self.assetsManifest.assets
		.filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
		.filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
		.map(asset => new Request(asset.url, { integrity: asset.hash }));
	await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
});

self.addEventListener('fetch', event => {
	var cacheControl = event.request.headers.get('cache-control');
	if (cacheControl !== 'no-cache')
	if (event.request.url.includes('GetImage')
		|| event.request.url.includes('GetAllSongs')
		|| event.request.url.includes('GetWholeSong')) {
		event.respondWith(
			caches.match(event.request)
			.then(function (response) {
				if (response) {
					return response;
				}
				caches.open(CACHE_NAME).then((cache) => {
					cache.add(event.request.url);
				});
				return fetch(event.request);
			})
		);
	}
});
