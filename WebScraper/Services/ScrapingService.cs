using System;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebScraper.Models;

namespace WebScraper.Services;

public class ScrapingService
{
    public List<Song> GetSongsInfoFromDocument(HtmlDocument document,string selector)
    {
        HtmlNodeCollection songRows = document.DocumentNode.SelectNodes(selector);
        if (songRows == null || !songRows.Any())
        {
            throw new InvalidOperationException(
                $"Не вдалося знайти жодного рядка пісні за селектором: '{selector}'. Перевірте контент HTML-документа."
            );
        }
        
        var songs = new List<Song>();
        
        foreach (var row in songRows)
        {
            var song = ExtractSongInfo(row);

            if (song != null && !string.IsNullOrWhiteSpace(song.Title) && !string.IsNullOrWhiteSpace(song.ArtistName))
            {
                songs.Add(song);
            }
        }

        Console.WriteLine($"Фактичних пісень вилучено: {songs.Count}");
        return songs;
    }
    
    public Song ExtractSongInfo(HtmlNode songRow)
    {
        var titleNode = songRow.SelectSingleNode(".//div[contains(@class, 'content')]//div[contains(@class, 'col1')]");
        var artistNode = songRow.SelectSingleNode(".//div[contains(@class, 'content')]//div[contains(@class, 'col2')]");
        var albumNode = songRow.SelectSingleNode(".//div[contains(@class, 'content')]//div[contains(@class, 'col3')]");
        var durationNode = songRow.SelectSingleNode( ".//div[contains(@class, 'content')]//div[contains(@class, 'col4')]//span");
        
        string title = titleNode?.InnerText.Trim();
        string artist = artistNode?.InnerText.Trim();
        string album = albumNode?.InnerText.Trim();
        string durationString = durationNode?.InnerText.Trim();

        TimeSpan duration = TimeSpan.Zero;
        
        if (!string.IsNullOrEmpty(durationString))
        {
            if (TimeSpan.TryParseExact(durationString, 
                    new string[] { "m\\:ss", "mm\\:ss", "h\\:mm\\:ss" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    out TimeSpan result))
            {
                duration = result;
            }
            else if (int.TryParse(durationString, out int totalSeconds))
            {
                duration = TimeSpan.FromSeconds(totalSeconds);
            }
        }
        
        return new Song()
        {
            Title = title,
            ArtistName = artist,
            AlbumName = album,
            Duration = duration
        };
    }
}