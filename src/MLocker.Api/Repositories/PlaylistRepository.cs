using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MLocker.Core.Models;

namespace MLocker.Api.Repositories
{
	public interface IPlaylistRepository
	{
		Task<Playlist> CreatePlaylist(string title);
	}

	public class PlaylistRepository : IPlaylistRepository
	{
		private readonly IConfig _config;

		public PlaylistRepository(IConfig config)
		{
			_config = config;
		}

		public async Task<Playlist> CreatePlaylist(string title)
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var playlistID = await connection.QuerySingleAsync<int>("INSERT INTO Playlists (Title) VALUES (@Title);SELECT CAST(SCOPE_IDENTITY() as int)", new {Title = title});
			var entity = new Playlist {PlaylistID = playlistID, Title = title};
			return entity;
		}
	}
}