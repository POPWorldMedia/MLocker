CREATE TABLE [dbo].[Songs](
	[FileID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
	[Title] [nvarchar](256) NOT NULL,
	[Artist] [nvarchar](256) NULL,
	[AlbumArtist] [nvarchar](256) NULL,
	[Album] [nvarchar](256) NULL,
	[Composer] [nvarchar](256) NULL,
	[Genre] [nvarchar](256) NULL,
	[Year] [int] NULL,
	[Track] [int] NULL,
	[TrackCount] [int] NULL,
	[Disc] [int] NULL,
	[DiscCount] [int] NULL,
	[Ticks] [bigint] NOT NULL,
	[PlayCount] [int] NOT NULL,
	[PictureMimeType] [nvarchar](50) NULL,
	[FileName] [nvarchar](256) NOT NULL,
	[FileType] [nvarchar](6) NOT NULL
)
GO
