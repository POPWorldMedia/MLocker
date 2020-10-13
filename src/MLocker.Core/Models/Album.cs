namespace MLocker.Core.Models
{
    public class Album
    {
        public string AlbumArtist { get; set; }
        public string Title { get; set; }
        public Song FirstSong { get; set; }
        public AlbumGroupingType AlbumGroupingType { get; set; }
    }
}