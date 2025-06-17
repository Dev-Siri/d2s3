using System.Text;
using System.Text.Json;
using d2s3.Models;

namespace d2s3.Services
{
  public class AiService(HttpClient? httpClient)
  {
    private readonly string _apiKey = "";

    public async Task<AiResponse> GenerateTextAsync(string prompt)
    {
      if (httpClient == null) return new AiResponse
      {
        Success = false,
        ResponseMessage = "Error: Failed to generate prompt."
      };

      var url = "https://api.openai.com/v1/completions";

      var requestData = new
      {
        model = "gpt-4.1",
        input = prompt,
        max_tokens = 150,
        temperature = 0.7
      };

      var jsonContent = JsonSerializer.Serialize(requestData);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var request = new HttpRequestMessage(HttpMethod.Post, url);
      request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
      request.Content = content;

      var response = await httpClient.SendAsync(request);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var completion = JsonSerializer.Deserialize<JsonElement>(responseContent);

        Console.WriteLine(completion);

        var responseText = completion.GetProperty("choices")[0].GetProperty("text").GetString();

        if (responseText == null) return new AiResponse
        {
          Success = false,
          ResponseMessage = "Error: Prompt generated was empty."
        };

        return new AiResponse
        {
          Success = true,
          ResponseMessage = responseText,
        };
      }
      else
      {
        return new AiResponse
        {
          Success = false,
          ResponseMessage = "Error: Could not generate prompt.",
        };
      }
    }
  }
}