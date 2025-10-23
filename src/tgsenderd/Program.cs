using System.Net;
using tgsenderd.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
var logger = app.Logger;

var tgToken = Environment.GetEnvironmentVariable("TG_TOKEN");
var tgChatId = Environment.GetEnvironmentVariable("TG_CHAT_ID");

if (string.IsNullOrEmpty(tgToken) || string.IsNullOrEmpty(tgChatId))
{
    const string message = "Environment variables TG_TOKEN and TG_CHAT_ID must be set.";
    Console.WriteLine(message);
    logger.LogCritical(message);
    return;
}

var tgSendUrl = $"https://api.telegram.org/bot{WebUtility.UrlEncode(tgToken)}/sendMessage";
var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

app.MapGet("/health", () => Results.Json(new Response(), statusCode: StatusCodes.Status200OK));

app.MapPost("/tg/send", async (HttpRequest req) =>
    {
        string body;

        using (var sr = new StreamReader(req.Body))
        {
            body = await sr.ReadToEndAsync();
        }

        if (string.IsNullOrEmpty(body))
            return Results.Json(new Response("Request body cannot be empty."),
                statusCode: StatusCodes.Status400BadRequest);


        var payload = new
        {
            chat_id = tgChatId,
            text = body
        };

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(tgSendUrl, payload);
        }
        catch (Exception e)
        {
            const string message = "Failed to call Telegram API";
            logger.LogError(e, message);
            return Results.Json(
                new Response(message),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!response.IsSuccessStatusCode)
        {
            var respText = await response.Content.ReadAsStringAsync();
            logger.LogError("Telegram API returned {Status}: {Body}", response.StatusCode, respText);

            return Results.Json(
                new Response("Telegram API error"),
                statusCode: StatusCodes.Status502BadGateway
            );
        }

        logger.LogInformation("Forwarded message to Telegram from {RemoteIp}",
            req.HttpContext.Connection.RemoteIpAddress);

        return Results.Json(
            new Response(),
            statusCode: StatusCodes.Status200OK);
    }
);

app.Run("http://0.0.0.0:7850");