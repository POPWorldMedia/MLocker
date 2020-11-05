# MLocker
This project takes an experimental shot at building a personal music locker. All the services are shifting to subscriber models that don't care about the library that you've purchased and built. Those services don't have all the music you do, and when it's "free" there are ads. Amazon shutdown their locker service a few years ago, and now Google has forced YouTube Music on to us, and it's a trainwreck. So what does one do? Builds their own service, of course.  

The app uses an aspnetcore API, storing the music files in Azure Storage and the metadata and playlists in SQL. (I was going to use something else, but I have a wholly underutilized SQL pool. I was going to use Azure Functions for the API, but I have a wholly underutilized app service, too.) The front-end is a Blazor client, but I'm confident that I can probably whip up a phone client or two.

![image](https://user-images.githubusercontent.com/2114255/98284863-bdbc5780-1f6f-11eb-9aa0-7c563d78b1f0.png)

## Roadmap
This is really an exercise in experimentation, but using it myself daily, I do want to formally make some releases. I think v1 will nail down the rest of the basic functionality, like editing playlists, and then a subsequent release will take a stab at mobile apps that will cache the music locally.

## Database
Change the startup properties of the `MLocker.Database` project to point to a database with a connection string, then run it. The default in source is looking for a database called `MLocker` on `localhost`. This uses `dbup` to keep the schema updated.

## Web client
* Runs as a Blazor app, from the same project as the API, for convenience.

## Storage
* You need a container called music in the storage account you configure. The connection string is in the 'appsettings.json'.

## Notes and observations
* The client will ask you for an API key if you run it and it doesn't have one saved. The one stored in the API's config is `123`, but of course you can change it.
* Running under IIS requires changing max request size for uploads. This is not the case for Linux or Linux app services.
* There's probably a cleaner way to add the API key header to requests from the client, though there aren't many calls anyway.
* Fun fact: I'm loading the entire song index on start. For a thousand songs, this is around 2.5 MB, but it's worth the two or three seconds to make everything else nearly instant when searching for music.
* Related fact: Initially I had compression on to squeeze that initial load, but because it then has to compress the output before streaming it out, this is actually slower.
* Neat thing: the MVC bits already have the ability to turn on content negotiation, meaning the browser will generally just download enough song to play it, and get the rest when it needs more. Not sure if, via the `Stream`s, this passes through to the Azure storage calls (which I know also will do the negotiation).
* Blazor is suprisingly great for manipulating the DOM wihtout having to get JS interop calls involved. Note the expansion of the "now playing" album cover when you click on it. That's all CSS and Blazor!
* SQL is definitely overkill for this. Playlists can certainly be documents, and songs probably can too.
* It feels like there should be a way to turn an API project into a series of Azure Functions, but there isn't.
* Shuffle is achieved by generating GUID's, which CS nerds insist is "wrong" because they're not random. I would aruge they're more than adequate, but also that GUID's are not random on Windows. Running everywhere else, including the Mono implementation in the browser, they are in fact random, seeded with `RNGCryptoServiceProvider`. But even if they weren't, we're shuffling songs, not powering a casino game. Y'all need to be pragmatic.
* HTTP calls seem to be routed though Javascript interop, relying on the browser to make the calls. It appears that there's a memory leak there, because I can crash the app on my comprooder when attempting to upload 8,000 songs. The tab uses 22 gigabytes of memory and fails on Chrome/Edge around 2,500 songs. Firefox grows similarly, but seems to evacuate large chunks of memory now and then, and sometimes can make the whole run. Not sure if it's a Blazor problem or browser problem, but [I filed a bug](https://github.com/dotnet/aspnetcore/issues/27023). There's a console app in the solution to upload large batches.
