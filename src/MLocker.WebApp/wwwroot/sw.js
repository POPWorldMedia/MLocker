const CACHE_NAME = 'music-image-cache';

self.addEventListener('install', event => event.waitUntil(onInstall(event)));

async function onInstall(event) {
	await self.skipWaiting();
}

self.addEventListener('fetch', event => {
	let cacheControl = event.request.headers.get('cache-control');
	if (cacheControl === 'no-cache')
		caches.open(CACHE_NAME)
			.then((cache) => {
				return cache.delete(event.request.url);
			});
	if (event.request.url.includes('GetImage')
		|| event.request.url.includes('GetWholeSong')) {
		event.respondWith(handleCacheableRequest(event.request));
	}
});

async function handleCacheableRequest(request) {
	const cache = await caches.open(CACHE_NAME);
	let response = await cache.match(request.url);
	if (!response) {
		const networkResponse = await fetch(request.url);
		await cache.put(request.url, networkResponse.clone());
		response = networkResponse;
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
