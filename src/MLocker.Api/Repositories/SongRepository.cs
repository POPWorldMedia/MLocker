using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MLocker.Core.Models;

namespace MLocker.Api.Repositories
{
    public interface ISongRepository
    {
        Task SaveSong(Song song);
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
    }
}