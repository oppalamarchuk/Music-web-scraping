using System;
using test_scraping;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using WebScraper.Services;

namespace WebScraper.Models;

public class MainWindowViewModel
{
    private readonly IHtmlLoaderService _loader;
    private readonly ScrapingService _scraper;
    
    public ObservableCollection<Song> Songs { get; } = new ObservableCollection<Song>();

    public MainWindowViewModel(IHtmlLoaderService loader, ScrapingService scraper)
    {
        _loader = loader;
        _scraper = scraper;
        
        LoadSongsAsync(); 
    }

    private async Task LoadSongsAsync(string url = "https://music.amazon.com/playlists/B01M11SBC8")
    {
        try
        {
            var page = await _loader.LoadHtml(url, "Accept cookies").ConfigureAwait(false);

            string selector = "//music-image-row";
            var newSongs = _scraper.GetSongsInfoFromDocument(page, selector); 
            
            await Dispatcher.UIThread.InvokeAsync(() => 
            {
                foreach (var song in newSongs)
                {
                    Songs.Add(song);
                }
                Console.WriteLine($"Завантажено пісень: {Songs.Count}");
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час завантаження: {ex.Message}");
        }
    }
}