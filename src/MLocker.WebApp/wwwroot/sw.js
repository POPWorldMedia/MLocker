importScripts('./service-worker-assets.js');

const CACHE_NAME = 'music-image-cache';
const APP_SHELL_CACHE_PREFIX = 'app-shell-cache-';
const APP_SHELL_CACHE_NAME = `${APP_SHELL_CACHE_PREFIX}${self.assetsManifest.version}`;

// Matches the icon baked into the .coverPlaceholder CSS class, so a missing/uncached
// album art image falls back to looking like the app's own "no art" placeholder.
const PLACEHOLDER_IMAGE_SVG = "<svg viewBox='0 0 16 16' xmlns='http://www.w3.org/2000/svg'><rect width='16' height='16' fill='#ddd'/><path fill='#ccc' fill-rule='evenodd' d='M10.804 8L5 4.633v6.734L10.804 8zm.792-.696a.802.802 0 0 1 0 1.392l-6.363 3.692C4.713 12.69 4 12.345 4 11.692V4.308c0-.653.713-.998 1.233-.696l6.363 3.692z' clip-rule='evenodd'/></svg>";

self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));

async function onInstall(event) {
	const cache = await caches.open(APP_SHELL_CACHE_NAME);
	const assets = self.assetsManifest.assets.filter(asset => !asset.url.match(/\.gz$/));

	// cache.addAll() is all-or-nothing - one unexpectedly-missing asset (e.g. a stray
	// npm package file that isn't actually served) would otherwise sink the whole precache
	// and leave the app shell with no offline fallback at all. Cache what we can instead.
	await Promise.all(assets.map(async asset => {
		try {
			const response = await fetch(new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
			if (response.ok)
				await cache.put(asset.url, response);
		} catch (err) {
			console.warn(`Service worker: failed to precache ${asset.url}`, err);
		}
	}));

	await self.skipWaiting();
}

async function onActivate(event) {
	const cacheKeys = await caches.keys();
	await Promise.all(cacheKeys
		.filter(key => key.startsWith(APP_SHELL_CACHE_PREFIX) && key !== APP_SHELL_CACHE_NAME)
		.map(key => caches.delete(key)));
	await self.clients.claim();
}

self.addEventListener('fetch', event => {
	if (event.request.method !== 'GET')
		return;

	let cacheControl = event.request.headers.get('cache-control');
	if (cacheControl === 'no-cache')
		caches.open(CACHE_NAME)
			.then((cache) => {
				return cache.delete(event.request.url);
			});

	if (event.request.url.includes('GetImage')
		|| event.request.url.includes('GetWholeSong')) {
		event.respondWith(handleCacheableRequest(event.request));
		return;
	}

	event.respondWith(handleAppShellRequest(event.request));
});

async function handleAppShellRequest(request) {
	const cache = await caches.open(APP_SHELL_CACHE_NAME);
	if (request.mode === 'navigate') {
		const shell = await cache.match('index.html');
		if (shell)
			return shell;
	}
	// Precached asset keys don't carry the cache-busting "?v=" query strings that
	// index.html requests them with, so ignore the query string when matching.
	const cached = await cache.match(request, { ignoreSearch: true });
	return cached || fetch(request);
}

async function handleCacheableRequest(request) {
	const cache = await caches.open(CACHE_NAME);
	let response = await cache.match(request.url);
	if (!response) {
		try {
			const networkResponse = await fetch(request.url);
			await cache.put(request.url, networkResponse.clone());
			response = networkResponse;
		} catch (err) {
			// Uncached art with no network to fetch it - fall back to the placeholder icon
			// rather than a broken image. Audio failures should still propagate as errors,
			// since that's what drives the offline-playback modal.
			if (request.url.includes('GetImage'))
				return new Response(PLACEHOLDER_IMAGE_SVG, { headers: { 'Content-Type': 'image/svg+xml' } });
			throw err;
		}
	}

	// Cache Storage can't reliably serve Range requests on its own (Safari doesn't
	// support it at all), so for the whole-song endpoint we slice the cached body
	// ourselves and return a proper 206 for whatever range the <audio> element asked for.
	const rangeHeader = request.headers.get('range');
	if (!rangeHeader || !request.url.includes('GetWholeSong'))
		return response;

	return buildPartialResponse(response, rangeHeader);
}

async function buildPartialResponse(fullResponse, rangeHeader) {
	const buffer = await fullResponse.clone().arrayBuffer();
	const total = buffer.byteLength;
	const match = /bytes=(\d*)-(\d*)/.exec(rangeHeader);
	if (!match)
		return fullResponse;

	let start = match[1] ? parseInt(match[1], 10) : 0;
	let end = match[2] ? parseInt(match[2], 10) : total - 1;
	if (isNaN(start) || start < 0) start = 0;
	if (isNaN(end) || end > total - 1) end = total - 1;

	if (start > end) {
		return new Response(null, {
			status: 416,
			statusText: 'Range Not Satisfiable',
			headers: { 'Content-Range': `bytes */${total}` }
		});
	}

	const slice = buffer.slice(start, end + 1);
	const headers = new Headers(fullResponse.headers);
	headers.set('Content-Range', `bytes ${start}-${end}/${total}`);
	headers.set('Content-Length', slice.byteLength.toString());
	headers.set('Accept-Ranges', 'bytes');

	return new Response(slice, { status: 206, statusText: 'Partial Content', headers });
}
