using System.Threading.Tasks;
using DotNetEnv;
using Google.GenAI;
using Google.GenAI.Types;

namespace DesktopPet.Classes;

class Gemini
{
    string? _apiKey;
    Client _client;
    public Gemini()
    {
        Env.Load();
        _apiKey = "";
        _apiKey = System.Environment.GetEnvironmentVariable("GeminiApiKey");
        _client = new Client(apiKey: _apiKey); 
    }
    

    public async Task<string> GetResponse(string promt)
    {
        var client = _client;
        try
        {
            var response = await _client.Models.GenerateContentAsync(
                model: "models/gemini-2.5-flash", 
                contents: promt
            );

            if (response?.Candidates?[0]?.Content?.Parts?[0] != null)
            {
                string result = response.Candidates[0].Content.Parts[0].Text;
                if (string.IsNullOrEmpty(result))
                {
                    return "Received an empty response.";
                }
                return result;   
            }
            return "Couldn't find a response.";
        }
        catch (System.Exception)
        {
            return "An error occurred while fetching the response.";
        }
    }
}