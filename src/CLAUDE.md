# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MLocker is a browser-based personal music locker built with ASP.NET Core and Blazor WebAssembly. The application stores music files in Azure Storage and metadata/playlists in SQL Server, serving as a self-hosted alternative to commercial music streaming services.

**Key characteristic**: The entire song library metadata is loaded on startup and cached client-side, making navigation and search instantaneous even with libraries of 8,000+ songs.

## Solution Structure

The solution consists of 5 projects:

- **MLocker.Core** (.NET Standard 2.1): Shared models and service interfaces used by both API and WebApp
- **MLocker.Api** (.NET 10): ASP.NET Core API backend that handles song uploads, streaming, playlist management, and serves the Blazor WebAssembly app
- **MLocker.WebApp** (.NET 10): Blazor WebAssembly client providing the music player interface
- **MLocker.Database** (.NET 10): Console app for database migrations using DbUp with embedded SQL scripts
- **MLocker.Bulk** (.NET 10): Console app for bulk uploading songs (workaround for browser memory limitations)

## Common Commands

### Building and Running

```bash
# Build entire solution
dotnet build MLocker.sln

# Run the API (also serves the Blazor WebApp)
cd MLocker.Api
dotnet run

# Build and watch WebApp frontend assets (Bootstrap, Popper.js)
cd MLocker.WebApp
npm install
npx gulp
```

### Database Setup

The Database project uses DbUp to run embedded SQL migration scripts in order.

```bash
cd MLocker.Database
dotnet run "server=localhost;Database=mlocker;Trusted_Connection=True;TrustServerCertificate=True;"
```

If the connection string has spaces, wrap it in quotes.

### Running Individual Projects

```bash
# Run API only
dotnet run --project MLocker.Api

# Run bulk upload tool
dotnet run --project MLocker.Bulk
```

## Architecture

### API Structure (MLocker.Api)

The API is a traditional ASP.NET Core application with:
- **Controllers**: REST endpoints for songs, playlists, uploads
- **Repositories**: Data access layer using Dapper for SQL and Azure.Storage.Blobs for file storage
- **Services**: Business logic including file parsing (TagLibSharp for MP3 metadata), playlist management
- **Authentication**: Custom ApiAuth attributes checking for API keys in request headers (not OAuth/JWT)

The API serves both as a REST backend and as the static file host for the Blazor WebAssembly app (configured in Program.cs with `UseBlazorFrameworkFiles()` and `MapFallbackToFile("index.html")`).

### WebApp Structure (MLocker.WebApp)

Blazor WebAssembly SPA with:
- **Pages**: Razor components for Albums, Artists, Playlists, Songs, Upload, Settings
- **Shared**: Reusable components including the audio player
- **Repositories**: HTTP clients calling the API, plus LocalStorageRepository for browser cache
- **Services**: Client-side business logic including MusicService (manages song library state), PlayerService (audio playback), PlaylistService

**Critical pattern**: The app loads all song metadata once on startup, caches it in browser local storage with a version GUID. Subsequent loads check if the cached version is current before making a network request.

### Core Models (MLocker.Core)

Key domain models shared between API and WebApp:
- **Song**: Individual track with metadata (artist, album, title, duration, etc.)
- **Album**: Grouped by AlbumArtist + Album fields
- **Playlist**: User-created song collections stored as serialized JSON in SQL
- **AlbumGroupingType**: Enum for different album view modes

**Important**: Albums are grouped by `AlbumArtist` + `Album` fields together. The upload process populates `AlbumArtist` from `Artist` if empty to ensure proper grouping.

### Data Flow

1. **Upload**: Client sends MP3 → API parses metadata with TagLibSharp → Stores blob in Azure Storage → Saves metadata to SQL
2. **Library Load**: Client requests version GUID → If cached version matches, load from local storage; else fetch full song list → Cache locally
3. **Playback**: Client requests song stream → API proxies from Azure blob storage with range request support → Browser plays audio

### Frontend Build Process

The WebApp uses Gulp to copy Bootstrap and Popper.js from node_modules to wwwroot/lib. This runs via the `AfterBuild` binding in gulpfile.js.

## Configuration

The API requires three settings (configure via appsettings.json or override at infrastructure level):

- `ConnectionString`: SQL Server database connection string
- `StorageConnectionString`: Azure Storage connection string (use `UseDevelopmentStorage=true` for Azurite local emulator)
- `ApiKey`: Authentication key that clients must provide in request headers

The storage account requires a container named `music`.

## Development Notes

- **WebApp serves from API**: In development, the API project has `UseWebAssemblyDebugging()` enabled and serves the Blazor app, so only run MLocker.Api
- **Album metadata**: The AlbumArtist field is critical for proper album grouping. Soundtracks should have "Various Artists" in AlbumArtist while individual artists go in Artist field
- **Bulk upload workaround**: Browsers have memory limitations when uploading thousands of songs via HTTP. Use MLocker.Bulk console app for large batches
- **No AOT compilation**: AOT is disabled for the WebApp as it breaks serialization in iOS Safari
- **Song streaming**: The API properly handles HTTP range requests for efficient audio streaming without loading entire files into memory
- **Cache strategy**: Album art is cached via browser Cache API and service worker. Song metadata uses local storage with version-based invalidation