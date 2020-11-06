using System.Collections.Generic;

namespace MLocker.Core.Models
{
    public class Album
    {
        public string AlbumArtist { get; set; }
        public string Title { get; set; }
        public Song FirstSong { get; set; }
        public AlbumGroupingType AlbumGroupingType { get; set; }
        public List<Song> Songs { get; set; }
    }
}