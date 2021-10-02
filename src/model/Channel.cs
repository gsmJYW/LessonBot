using System;

namespace IzoneBot.src.model
{
    public class Channel
    {
        public Item[] items { get; set; }
    }

    public class Item
    {
        public Snippet snippet { get; set; }
    }

    public class Snippet
    {
        public string title { get; set; }
        public DateTime publishedAt { get; set;  }
        public ResourceId resourceId { get; set; }
    }

    public class ResourceId
    {
        public string videoId { get; set; }
    }
}
