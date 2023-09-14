using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ctwebapp.Pages;

public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // this all needs to be client side, so there is no need to process the request in Azure
            // and the person attempting to abuse my API gets a nice speedy response directly from an edge node near them
            string debug = string.Empty;

            var form = await Request.ReadFormAsync();

            var token = form["cf-turnstile-response"];

            var ip = Request.HttpContext.Connection.RemoteIpAddress!.ToString();

            var formData = new MultipartFormDataContent();

            var SECRET_KEY = "1x0000000000000000000000000000000AA";

            formData.Add(new StringContent(SECRET_KEY), "secret");
            formData.Add(new StringContent(token!), "response");
            formData.Add(new StringContent(ip), "remoteip");

            var url = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

            var client = new HttpClient();

            var response = await client.PostAsJsonAsync(url, formData);

            if (!response.IsSuccessStatusCode)
            {
                debug += JsonSerializer.Serialize(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return Page();
    }
}


