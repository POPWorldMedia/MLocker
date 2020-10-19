# MLocker

This project takes an experimental shot at building a personal music locker. All the services are shifting to subscriber models that don't care about the library that you've purchased and built. Those services don't have all the music you do, and when it's "free" there are ads. Amazon shutdown their locker service a few years ago, and now Google has forced YouTube Music on to us, and it's a trainwreck. So what does one do? Builds their own service, of course.  

The app uses an aspnetcore API, storing the music files in Azure Storage and the metadata and playlists in SQL. (I was going to use something else, but I have a wholly underutilized SQL pool. I was going to use Azure Functions for the API, but I have a wholly underutilized app service, too.) The front-end is a Blazor client, but I'm confident that I can probably whip up a phone client or two.

## Database
Change the startup properties of the `MLocker.Database` project to point to a database with a connection string, then run it. The default in source is looking for a database called `MLocker` on `localhost`. This uses `dbup` to keep the schema updated.

## Web client
* Runs as a Blazor app, from the same project as the API, for convenience.

## Storage
* You need a container called music.

## Notes
* The client will ask you for an API key if you run it and it doesn't have one saved. The one stored in the API's config is `123`, but of course you can change it.
* Running under IIS requires changing max request size for uploads.
* There's probably a cleaner way to add the API key header to requests from the client, though there aren't many calls anyway.
* Fun fact: I'm loading the entire song index on start. Compressed, I'm estimating that 10,000 songs would be a payload around 750k. Whatever, that's not much these days!
