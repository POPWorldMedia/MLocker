using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MLocker.Core.Models;

namespace MLocker.Api.Repositories
{
    public interface ISongRepository
    {
        Task SaveSong(Song song);
        Task<IEnumerable<Song>> GetAll();
        Task<Song> GetSong(int fileID);
        Task IncrementPlayCount(int fileID);
    }

    public class SongRepository : ISongRepository
    {
        private readonly IConfig _config;

        public SongRepository(IConfig config)
        {
            _config = config;
        }

        public async Task SaveSong(Song song)
        {
            await using var connection = new SqlConnection(_config.ConnectionString);
            await connection.ExecuteAsync("INSERT INTO Songs (Title, Artist, AlbumArtist, Album, Composer, Genre, Year, Track, TrackCount, Disc, DiscCount, Ticks, PlayCount, PictureMimeType, FileName, FileType) VALUES (@Title, @Artist, @AlbumArtist, @Album, @Composer, @Genre, @Year, @Track, @TrackCount, @Disc, @DiscCount, @Ticks, @PlayCount, @PictureMimeType, @FileName, @FileType)", song);
        }

        public async Task<IEnumerable<Song>> GetAll()
        {
            await using var connection = new SqlConnection(_config.ConnectionString);
            var allSongs = await connection.QueryAsync<Song>("SELECT * FROM Songs");
            return allSongs;
        }

        public async Task<Song> GetSong(int fileID)
        {
            await using var connection = new SqlConnection(_config.ConnectionString);
            var song = await connection.QuerySingleOrDefaultAsync<Song>("SELECT * FROM Songs WHERE FileID = @FileID", new { FileID = fileID});
            return song;
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        await using var connection = new SqlConnection(_config.ConnectionString);
	        await connection.ExecuteScalarAsync("UPDATE Songs SET PlayCount = PlayCount + 1 WHERE FileID = @FileID", new { FileID = fileID});
        }
    }
}