using System;

namespace WebScraper.Models;

public record Song{
    public string Title { get; set; }
    public string ArtistName { get; set; }
    public string AlbumName { get; set; }
    public TimeSpan Duration { get; set; }
}
