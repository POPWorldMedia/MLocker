# MLocker

This project takes an experimental shot at building a personal music locker. All the services are shifting to subscriber models that don't care about the library that you've purchased and built. Those services don't have all the things you do. They put ads in there.

## Database
You can probably use something that's not a relational database here, but it's just easier to do playlists this way. Change the startup properties of the `MLocker.Database` project to point to a database, then run it. The default in source is looking for a database called `MLocker` on `localhost`. This uses `dbup` to keep the schema updated.

## Web client
* Runs as a Blazor app, from the same project as the API, for convenience.

## Storage

* You need a container called music.

## Notes

* Running under IIS requires changing max request size for uploads.
* There's probably a cleaner way to add the API key header to requests from the client, though there aren't many calls anyway.
