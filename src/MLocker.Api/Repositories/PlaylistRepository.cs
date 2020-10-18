using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MLocker.Core.Models;

namespace MLocker.Api.Repositories
{
	public interface IPlaylistRepository
	{
		Task<PlaylistDefinition> CreatePlaylistDefinition(string title);
		Task CreatePlaylistFile(PlaylistFile playlistFile);
		Task<IEnumerable<PlaylistDefinition>> GetAllPlaylistDefinitions();
		Task<IEnumerable<PlaylistFile>> GetAllPlaylistFiles();
	}

	public class PlaylistRepository : IPlaylistRepository
	{
		private readonly IConfig _config;

		public PlaylistRepository(IConfig config)
		{
			_config = config;
		}

		public async Task<PlaylistDefinition> CreatePlaylistDefinition(string title)
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var playlistID = await connection.QuerySingleAsync<int>("INSERT INTO Playlists (Title) VALUES (@Title);SELECT CAST(SCOPE_IDENTITY() as int)", new {Title = title});
			var entity = new PlaylistDefinition { PlaylistID = playlistID, Title = title};
			return entity;
		}
		
		public async Task CreatePlaylistFile(PlaylistFile playlistFile)
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			await connection.ExecuteAsync("INSERT INTO PlaylistFiles (PlaylistID, FileID, SortOrder) VALUES (@PlaylistID, @FileID, @SortOrder)", playlistFile);
		}

		public async Task<IEnumerable<PlaylistDefinition>> GetAllPlaylistDefinitions()
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var result = await connection.QueryAsync<PlaylistDefinition>("SELECT * FROM Playlists");
			var list = result.ToList();
			return list;
		}

		public async Task<IEnumerable<PlaylistFile>> GetAllPlaylistFiles()
		{
			await using var connection = new SqlConnection(_config.ConnectionString);
			var result = await connection.QueryAsync<PlaylistFile>("SELECT * FROM PlaylistFiles");
			var list = result.ToList();
			return list;
		}
	}
}