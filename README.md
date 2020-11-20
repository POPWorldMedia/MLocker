# MLocker
This project takes an experimental shot at building a personal music locker. All the services are shifting to subscriber models that don't care about the library that you've purchased and built. Those services don't have all the music you do, and when it's "free" there are ads. Amazon shutdown their locker service a few years ago, and now Google has forced YouTube Music on to us, and it's a trainwreck. So what does one do? Builds their own service, of course.  

The app uses an aspnetcore API, storing the music files in Azure Storage and the metadata and playlists in SQL. (I was going to use something else, but I have a wholly underutilized SQL pool. I was going to use Azure Functions for the API, but I have a wholly underutilized app service, too.) The front-end is a Blazor client, but I'm confident that I can probably whip up a phone client or two.

![image](https://user-images.githubusercontent.com/2114255/98284863-bdbc5780-1f6f-11eb-9aa0-7c563d78b1f0.png)

## Roadmap
This is really an exercise in experimentation, but using it myself daily, I do want to formally make some releases. I think v1 is close, as there aren't any major features I want to add. About 2,000 tracks played back so far on my instance!

## Features
* Loads all library metadata up front, caches it if it hasn't changed. Navigating music and searching in lists is instantaneous because you have all the data! Tested with library of about 8k songs. Normal data hit for start with no new content is about 11kb.
* Download playlists and albums to your device while on wifi, to reduce data use on cellular.
* Album art automatically cached on your device.
* Supports `MediaSession` API, so controls and current track appear in Android lock screen and Windows media controls. (iOS support, not so much.)
* From any song list, context menu allows you to add song to most recent playlist, another playlist, play the song next, add it to the queue, go to the album or artist.
* Add entire albums or playlists to the queue.
* Shuffle all the things, instantly.
* Artist view shows all the albums, and all the songs (you can shuffle those, too).
* Responsive view works in your desktop browser and your mobile device.
* Install it as a progressive web app, with an icon in your taskbar or launcher. Works great with Chrome/Edge and Firefox, on Android and Windows. (iOS, not so much.)

## Getting started
* First off, I'm not a lawyer, and this isn't legal advice, but existing case law seems to suggest that you can store your own music that you own in the cloud, so this shouldn't be materially different than doing so with Amazon or Google. That said, don't share it and give your API key to everyone. Also, there is no defensive coding against concurrency issues, so your playlists will get written over by another user if you're editing them at the same time.
* I've got the API and the Blazor web app serving out of the same site. You could separate these if you like, but it's just for you, so I'm not sure what you would gain.
* `appsettings.json` has three settings in the API project, and you should override these at the infrastructure level (i.e., an Azure App Service):
  * `ConnectionString`: the SQL database that stores the song metadata and playlists.
  * `StorageConnectionString`: the connection string to your Azure storage. You get this from the Azure portal. Use `UseDevelopmentStorage=true` for the local emulator.
  * `ApiKey`: A string that the client needs to connect.
* Your storage account needs a container called `music`, and the app won't create this for you.
* Change the startup properties of the `MLocker.Database` project to point to a database with a connection string, then run it. Remember this is a command line, so if your connection string has spaces, surround it in quotes. The default in source is looking for a database called `MLocker` on `localhost`. This uses `dbup` to keep the schema updated.

## Notes and observations
* The client will ask you for an API key if you run it and it doesn't have one saved. The one stored in the API's config is `123`, but of course you can change it.
* Running under IIS requires changing max request size for uploads. This is not the case for Linux or Linux app services.
* There's probably a cleaner way to add the API key header to requests from the client, though there aren't many calls anyway.
* Fun fact: I'm loading the entire song index on start. For 8k songs, this is around 2.5 MB, but it's worth the two or three seconds to make everything else nearly instant when searching for music. The playlists, albums and artists are collectively populated based on the song data in under 300ms on my machine for 8k songs.
* Related fact: Initially I had compression on to squeeze that initial load, but because it then has to compress the output before streaming it out, this is actually slower.
* The browser cache API is very cool. Combined with a Javascript service worker, the app caches all of your album art. If you scroll through all of your songs, they'll all be stored for you.
* To help reudce bandwidth and startup time, it hits the server for a version string (a GUID) to see if your cached song list is current. If it is, it loads it from there in a few milleseconds instead of two or three seconds from the Internets for a big list.
* iOS absolutely sucks for adhering to standards that make PWA's awesome, not the least of which is that it won't play media unless you touch something, which is not ideal. However, it works flawlessly on your Android phone in Chrome, in the background while the phone is locked. It even fully supports the `MediaSession.metadata` API, so your lock screen shows all the controls, song info and image. It feels close to native on Android. That's pretty great.
* Neat thing: the MVC/API bits already have the ability to turn on content negotiation, meaning the browser will generally just download enough song to play it, and get the rest when it needs more. Not sure if, via the `Stream`s, this passes through to the Azure storage calls (which I know also will do the negotiation).
* Blazor is suprisingly great for manipulating the DOM wihtout having to get JS interop calls involved. Note the expansion of the "now playing" album cover when you click on it. That's all CSS and Blazor!
* SQL is definitely overkill for this. Playlists can certainly be documents, and songs probably can too.
* I did try using Azure Functions instead of an API project, but on the consumption plan, response times were a little unpredictable, since I'm the only user.
* Shuffle is achieved by generating GUID's, which CS nerds insist is "wrong" because they're not random. I would aruge they're more than adequate, but also that GUID's are not random on Windows. Running everywhere else, including the Mono implementation in the browser, they are in fact random, seeded with `RNGCryptoServiceProvider`. But even if they weren't, we're shuffling songs, not powering a casino game. Y'all need to be pragmatic.
* HTTP calls seem to be routed though Javascript interop, relying on the browser to make the calls. It appears that there's a memory leak there, because I can crash the app on my comprooder when attempting to upload 8,000 songs. The tab uses 22 gigabytes of memory and fails on Chrome/Edge around 2,500 songs. Firefox grows similarly, but seems to evacuate large chunks of memory now and then, and sometimes can make the whole run. Not sure if it's a Blazor problem or browser problem, but [I filed a bug](https://github.com/dotnet/aspnetcore/issues/27023). There's a console app in the solution to upload large batches.
