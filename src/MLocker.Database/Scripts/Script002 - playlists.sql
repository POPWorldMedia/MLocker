CREATE TABLE [dbo].[Playlists](
	[PlaylistID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
	[Title] [nvarchar](256) NOT NULL
)
GO

CREATE TABLE [dbo].[PlaylistFile](
	[PlaylistID] [int] NOT NULL,
	[FileID] [int] NOT NULL
)
GO

CREATE CLUSTERED INDEX [IX_PlaylistFile_PlaylistID] ON [dbo].[PlaylistFile]
(
	[PlaylistID] ASC
)
GO

ALTER TABLE [dbo].[PlaylistFile]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistFile_FileID] FOREIGN KEY([FileID])
REFERENCES [dbo].[Songs] ([FileID])
GO

ALTER TABLE [dbo].[PlaylistFile] CHECK CONSTRAINT [FK_PlaylistFile_FileID]
GO

ALTER TABLE [dbo].[PlaylistFile]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistFile_PlaylistID] FOREIGN KEY([PlaylistID])
REFERENCES [dbo].[Playlists] ([PlaylistID])
GO

ALTER TABLE [dbo].[PlaylistFile] CHECK CONSTRAINT [FK_PlaylistFile_PlaylistID]
GO

