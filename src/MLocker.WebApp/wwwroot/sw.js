﻿var CACHE_NAME = 'music-image-cache';

self.addEventListener('install', event => {
		self.skipWaiting();
	});

self.addEventListener('fetch', event => {
	if (event.request.url.includes('GetImage')) {
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