using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MLocker.Api.Models;

namespace MLocker.Api.Repositories
{
	public interface IPlaylistRepository
	{
		Task<PlaylistEntity> CreatePlaylistEntity(string title);
		Task<IEnumerable<PlaylistEntity>> GetAllPlaylistEntities();
		Task UpdatePlaylist(PlaylistEntity playlistEntity);
	}

	public class PlaylistRepository : IPlaylistRepository
	{
		private readonly IConfig _config;

		public PlaylistRepository(IConfig config)
		{
			_config = config;
		}

		public async Task<PlaylistEntity> CreatePlaylistEntity(string title)
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var playlistID = await connection.QuerySingleAsync<int>("INSERT INTO Playlists (Title, SongsJson) VALUES (@Title, '');SELECT CAST(SCOPE_IDENTITY() as int)", new {Title = title});
			var entity = new PlaylistEntity { PlaylistID = playlistID, Title = title};
			return entity;
		}

		public async Task<IEnumerable<PlaylistEntity>> GetAllPlaylistEntities()
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var result = await connection.QueryAsync<PlaylistEntity>("SELECT * FROM Playlists");
			var list = result.ToList();
			return list;
		}

		public async Task UpdatePlaylist(PlaylistEntity playlistEntity)
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			await connection.ExecuteAsync("UPDATE Playlists SET Title = @Title, SongsJson = @SongsJson WHERE PlaylistID = @PlaylistID", playlistEntity);
		}
	}
}