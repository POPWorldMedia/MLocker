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
            await connection.ExecuteAsync("INSERT INTO Songs (Title, Artist, AlbumArtist, Album, Composer, Genre, Year, Track, TrackCount, Disc, DiscCount, Ticks, PlayCount, PictureMimeType, FileName) VALUES (@Title, @Artist, @AlbumArtist, @Album, @Composer, @Genre, @Year, @Track, @TrackCount, @Disc, @DiscCount, @Ticks, @PlayCount, @PictureMimeType, @FileName)", song);
        }

        public async Task<IEnumerable<Song>> GetAll()
        {
            await using var connection = new SqlConnection(_config.ConnectionString);
            var allSongs = await connection.QueryAsync<Song>("SELECT * FROM Songs");
            return allSongs;
        }
    }
}