var CACHE_NAME = 'music-image-cache';

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/, /^service-worker-assets\.js$/, /^sw\.js$/, /^manifest\.json$/];

async function onInstall(event) {
	self.skipWaiting();
	// Fetch and cache all matching items from the assets manifest
	const assetsRequests = self.assetsManifest.assets
		.filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
		.filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
		.map(asset => new Request(asset.url, { integrity: asset.hash }));
	await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
	// Delete unused caches
	const cacheKeys = await caches.keys();
	await Promise.all(cacheKeys
		.filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
		.map(key => caches.delete(key)));
}

self.addEventListener('fetch', event => {
	var cacheControl = event.request.headers.get('cache-control');
	if (cacheControl !== 'no-cache')
	if (event.request.url.includes('GetImage')
		|| event.request.url.includes('GetAllSongs')
		|| event.request.url.includes('GetAllPlaylistDefinitions')
		|| event.request.url.includes('GetWholeSong')) {
		event.respondWith(
			caches.match(event.request)
			.then(function (response) {
				if (response) {
					return response.clone();
				}
				caches.open(CACHE_NAME)
					.then((cache) => {
						cache.add(event.request);
				});
			})
		);
	}
});
