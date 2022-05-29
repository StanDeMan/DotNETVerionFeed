﻿namespace VersionsFeedService.VersionParser.Web;

public class Request
{
    public async Task<string> ReadVersionFeedService(string? uri = null)
    {
        string responseBody;

        if (string.IsNullOrEmpty(uri)) uri = new HtmlPage().VersionsFeedUri;

        try
        {
            var response = await new HttpClient().GetAsync(uri);
            response.EnsureSuccessStatusCode();

            responseBody = await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            responseBody = string.Empty;
        }

        return responseBody;
    }
}