using System;
using test_scraping;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Threading;
using WebScraper.Services;

namespace WebScraper.Models;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IHtmlLoaderService _loader;
    private readonly ScrapingService _scraper;
    
    public ObservableCollection<Song> Songs { get; } = new ObservableCollection<Song>();
    
    private Playlist _playlist;
    public Playlist Playlist
    {
        get => _playlist;
        private set
        {
            _playlist = value;
            OnPropertyChanged();
        }
    }
    public MainWindowViewModel(IHtmlLoaderService loader, ScrapingService scraper)
    {
        string alb = "https://music.amazon.com/albums/B073JC7DCF";
        string pla = "https://music.amazon.com/playlists/B01M11SBC8";
        
        _loader = loader;
        _scraper = scraper;
        
        LoadPlaylistAsync(pla ); 
    }
    
    private async Task LoadPlaylistAsync(string url)
    {
        var page = await _loader.LoadHtml(url, "Accept cookies").ConfigureAwait(false);
            
        var playlist = _scraper.GetPlaylist(page);
        string selector = "//music-image-row";
        var newSongs = _scraper.GetSongs(page, selector); 

     
        await Dispatcher.UIThread.InvokeAsync(() => 
        { 
            Playlist = playlist; 

            Songs.Clear();
            foreach (var song in newSongs)
            {
                Songs.Add(song);
            }
            Console.WriteLine($"Завантажено пісень: {Songs.Count}");
        });
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}