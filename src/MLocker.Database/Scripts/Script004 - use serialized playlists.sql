DELETE FROM PlaylistFiles
GO
DELETE FROM Playlists
GO

ALTER TABLE [Playlists]
ADD 
	[SongsJson] nvarchar(MAX) NOT NULL
GO

DROP TABLE [PlaylistFiles]
GO
