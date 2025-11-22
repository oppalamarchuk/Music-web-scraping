using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Playwright;

namespace test_scraping;

public interface IHtmlLoaderService
{
    Task<HtmlDocument> LoadHtml(string url, string cookieAcceptButtonText);
}
public class HtmlLoadService : IHtmlLoaderService
{
    public async Task<HtmlDocument> LoadHtml(string url,string cookieAcceptButtonText)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = 
                await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
                { StorageStatePath = "cookies.json" });
        IPage page = await context.NewPageAsync();
        
        await page.GotoAsync(url, new PageGotoOptions { Timeout = 120000 });
        
        await CloseCookieBannerAsync(page, cookieAcceptButtonText);
        
        string htmlContent =await page.ContentAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        return doc;
    }
    
    private async Task CloseCookieBannerAsync(IPage page, string cookieAcceptButtonText)
    {
        try
        {
            var cookieButton = page.GetByRole(
                AriaRole.Button, 
                new PageGetByRoleOptions { Name = cookieAcceptButtonText, Exact = false });
        
            Console.WriteLine("Спроба закрити cookie-банер...");
            await cookieButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await cookieButton.ClickAsync(new LocatorClickOptions { Timeout = 3000 }); 
        
            await cookieButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 5000 });
        
            Console.WriteLine("Cookie-банер успішно закрито.");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Кнопка Cookie не знайдена або не зникла після кліку. Продовжуємо.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час обробки cookie: {ex.Message}");
        }
    }
}