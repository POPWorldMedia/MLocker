using System;

namespace MLocker.Core.Models
{
	public class SongData : Song
	{
        public TimeSpan Length
        {
            get => new TimeSpan(Ticks);
            set => Ticks = value.Ticks;
        }
		public byte[] Picture { get; set; }
	}
}