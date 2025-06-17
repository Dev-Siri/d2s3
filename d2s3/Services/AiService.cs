using System.Text;
using System.Text.Json;
using d2s3.Models;

namespace d2s3.Services
{
  public class AiService(HttpClient? httpClient)
  {
    public readonly string url = "http://localhost:5203";

    public async Task<AiResponse> GenerateTextAsync(string prompt)
    {
      if (httpClient == null) return new AiResponse
      {
        Success = false,
        ResponseMessage = "Error: Failed to generate response."
      };

      try
      {
        var jsonContent = JsonSerializer.Serialize(new { prompt });
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/gemini")
        {
          Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync();
          var completion = JsonSerializer.Deserialize<JsonElement>(responseContent);

          var success = completion.GetProperty("success").GetBoolean();
          var responseText = completion.GetProperty("responseMessage").GetString();

          if (responseText == null || !success) return new AiResponse
          {
            Success = false,
            ResponseMessage = "Error: Unable to generate response."
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
            ResponseMessage = "Error: Could not generate response.",
          };
        }
      }
      catch
      {
        return new AiResponse
        {
          Success = false,
          ResponseMessage = "Error: An unhandled exception occured while sending request to API."
        };
      }
    }
  }
}