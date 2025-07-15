using SmartTuror.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

public class AIService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _deepSeekApiKey;

    public AIService(IUnitOfWork unitOfWork, IConfiguration configuration, HttpClient httpClient)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpClient = httpClient;

        // Load DeepSeek API Key from configuration
        _deepSeekApiKey = _configuration["DeepSeek:ApiKey"];

        // Set API Base URL
        _httpClient.BaseAddress = new Uri("https://api.deepseek.com/v1");

        _httpClient.Timeout = TimeSpan.FromSeconds(120);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_deepSeekApiKey}");
    }

    public async Task<string> GenerateExplanation(int questionId, int selectedAnswerId)
    {
        try
        {
            var question = await _unitOfWork.Question.GetAsync(q => q.Id == questionId, includeProperties: "Answers");
            if (question == null) return "Sorry, I couldn't find that question.";

            var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

            if (selectedAnswer == null || correctAnswer == null)
                return "Sorry, I couldn't process the answers.";

            var context = $"Question: {question.Text}\n" +
                          $"Correct answer: {correctAnswer.Text}\n" +
                          $"Selected answer: {selectedAnswer.Text}";

            return await CallDeepSeekApi(context, question.Text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating explanation: {ex.Message}");
            return "I couldn't generate an explanation. The correct answer is marked above.";
        }
    }

    private async Task<string> CallDeepSeekApi(string context, string questionText)
    {
        try
        {
            var correctAnswerText = context.Split('\n')[1].Replace("Correct answer: ", "");
            var selectedAnswerText = context.Split('\n')[2].Replace("Selected answer: ", "");

            var systemPrompt = $"You are a computer science tutor. Follow these rules strictly: " +
                               $"1. Do NOT provide an introduction. Start directly with the explanation. " +
                               $"2. In 1 - 2 sentences, explain why '{correctAnswerText}' is correct. " +
                               $"3. In 1 - 2 sentences, explain why '{selectedAnswerText}' is incorrect. " +
                               $"4. At the end, add a 'Learn more' section formatted like this: " +
                               $"'Learn more:\\n1. Link1\\n2. Link2\\n...' " +
                               $"5. Only include the links in the 'Learn more' section without any explanations. " +
                               $"6. Keep the response concise and to the point.";

            var requestData = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = $"Question: {questionText}\nCorrect: {correctAnswerText}\nSelected: {selectedAnswerText}" }
            },
                temperature = 0.7,
                max_tokens = 300
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);

            return result?.choices[0]?.message?.content ?? "No explanation generated.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI call failed: {ex}");
            return "Couldn't generate explanation (service error)";
        }
    }

}
