using System.IO;
using MLocker.Api.Models;
using MLocker.Core.Models;

namespace MLocker.Api.Services;

public interface IFileParsingService
{
    SongData ReadFileData(string fileName, byte[] fileBytes);
}

public class FileParsingService : IFileParsingService
{
    public SongData ReadFileData(string fileName, byte[] fileBytes)
    {
        var container = new MusicFileAbstraction(fileName, fileBytes);
        var file = TagLib.File.Create(container);
        byte[] pictureData = file.Tag.Pictures.Length > 0 ? file.Tag.Pictures?[0]?.Data?.Data : null;
        string pictureMimeTime = (file.Tag.Pictures.Length > 0 && pictureData?.Length > 0) ? file.Tag.Pictures?[0]?.MimeType : null;
        var fileType = Path.GetExtension(fileName);
        var albumArtist = string.IsNullOrEmpty(file.Tag.FirstAlbumArtist) ? file.Tag.FirstPerformer : file.Tag.FirstAlbumArtist;
        var fileData = new SongData
        {
            Title = file.Tag.Title ?? fileName,
            Artist = file.Tag.FirstPerformer,
            AlbumArtist = albumArtist,
            Album = file.Tag.Album,
            Composer = file.Tag.FirstComposer,
            Genre = file.Tag.FirstGenre,
            Year = (int?)file.Tag.Year,
            Track = (int?)file.Tag.Track,
            TrackCount = (int?)file.Tag.TrackCount,
            Disc = (int?)file.Tag.Disc,
            DiscCount = (int?)file.Tag.DiscCount,
            Length = file.Properties.Duration,
            Picture = pictureData,
            PictureMimeType = pictureMimeTime,
            FileName = fileName,
            FileType = fileType
        };
        return fileData;
    }
}