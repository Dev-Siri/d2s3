using System.Text;
using System.Text.Json;
using d2s3_server.Models;

namespace d2s3_server.Services
{
  class GeminiService
  {
    private readonly string _geminiApiKey = "";

    public GeminiService()
    {
      var key = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

      if (key == null)
      {
        Console.Error.WriteLine("ERR: Missing environment variable 'GEMINI_API_KEY'.");
        return;
      }

      _geminiApiKey = key;
    }

    public async Task<Response> GenerateTextAsync(List<OneModelMessage> messages)
    {
      var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_geminiApiKey}";
      var requestData = new GeminiRequest
      {
        Contents = messages
      };

      var jsonContent = JsonSerializer.Serialize(requestData);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var request = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = content
      };

      var httpClient = new HttpClient();
      var response = await httpClient.SendAsync(request);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var completion = JsonSerializer.Deserialize<JsonElement>(responseContent);

        var responseText = completion.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

        if (responseText == null) return new Response
        {
          Success = false,
          ResponseMessage = "Error: Prompt generated was empty."
        };

        return new Response
        {
          Success = true,
          ResponseMessage = responseText,
        };
      }
      else
      {
        return new Response
        {
          Success = false,
          ResponseMessage = "Error: Could not generate prompt.",
        };
      }
    }
  }
}