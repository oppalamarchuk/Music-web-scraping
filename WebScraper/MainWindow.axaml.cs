using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using test_scraping;
using WebScraper.Models;
using WebScraper.Services;
namespace WebScraper;

public partial class MainWindow : Window
{
    public ObservableCollection<Song> MyItems { get; } 

    public MainWindow()
    {
        InitializeComponent();

        var loader = new HtmlLoadService();
        var scraper = new ScrapingService();
        var viewModel = new MainWindowViewModel(loader, scraper);
        
        DataContext = viewModel;
    }
}