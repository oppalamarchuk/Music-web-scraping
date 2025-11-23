using System.Collections.Generic;

namespace WebScraper.Models;

public record Playlist
{
 public string Name { get; set; }
 public string AvatarUrl { get; set; }
 public string Description { get; set; }
}