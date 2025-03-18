using RestSharp;
using Newtonsoft.Json;

public class AIService
{
    private const string PythonAPIUrl = "http://localhost:5000/generate"; // URL of your Python API

    public string GenerateChapterContent(string prompt)
    {
        var client = new RestClient(PythonAPIUrl);
        var request = new RestRequest();
        request.Method = Method.Post;  // Use Method.Post here, with a capital 'P'
        request.AddJsonBody(new { prompt = prompt }); // Pass the prompt to the API

        var response = client.Execute(request);

        if (response.IsSuccessful)
        {
            // Assuming the response contains a list of generated text
            dynamic responseData = JsonConvert.DeserializeObject(response.Content);
            return responseData[0].generated_text.ToString(); // Adjust based on the structure of the response
        }

        return "Error generating content";
    }
}
