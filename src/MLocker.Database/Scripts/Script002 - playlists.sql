CREATE TABLE [dbo].[Playlists](
	[PlaylistID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
	[Title] [nvarchar](256) NOT NULL
)
GO

CREATE TABLE [dbo].[PlaylistFiles](
	[PlaylistID] [int] NOT NULL,
	[FileID] [int] NOT NULL,
	[SortOrder] [int] NOT NULL
)
GO

CREATE CLUSTERED INDEX [IX_PlaylistFiles_PlaylistID] ON [dbo].[PlaylistFiles]
(
	[PlaylistID] ASC
)
GO

ALTER TABLE [dbo].[PlaylistFiles]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistFiles_FileID] FOREIGN KEY([FileID])
REFERENCES [dbo].[Songs] ([FileID])
GO

ALTER TABLE [dbo].[PlaylistFiles] CHECK CONSTRAINT [FK_PlaylistFiles_FileID]
GO

ALTER TABLE [dbo].[PlaylistFiles]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistFiles_PlaylistID] FOREIGN KEY([PlaylistID])
REFERENCES [dbo].[Playlists] ([PlaylistID])
GO

ALTER TABLE [dbo].[PlaylistFiles] CHECK CONSTRAINT [FK_PlaylistFiles_PlaylistID]
GO

