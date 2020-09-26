namespace MLocker.Core.Models
{
    public class UploadContainer
    {
        public Song Song { get; set; }
        public byte[] File { get; set; }
        public byte[] Picture { get; set; }
    }
}