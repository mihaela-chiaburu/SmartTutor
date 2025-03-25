using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public class AIService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AIService(IUnitOfWork unitOfWork, IConfiguration configuration, HttpClient httpClient)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpClient = httpClient;

        // Add these configurations
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
        _httpClient.Timeout = TimeSpan.FromSeconds(120); // Extended timeout
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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

            var prompt = $"Why is '{correctAnswer.Text}' the correct answer to '{question.Text}' " +
                         $"and why is '{selectedAnswer.Text}' incorrect?";

            var response = await CallDistilBERTApi(context, prompt);

            return response ?? "Here's why this answer is incorrect: " +
                   $"The correct answer is '{correctAnswer.Text}' because {GenerateSimpleExplanation(question, correctAnswer)}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating explanation: {ex.Message}");
            return "I couldn't generate an explanation. The correct answer is marked above.";
        }
    }

    private async Task<string> CallDistilBERTApi(string context, string question)
    {
        try
        {
            // Extract the correct and selected answers from context
            var lines = context.Split('\n');
            if (lines.Length < 3)
            {
                return "Invalid question format";
            }

            var questionText = lines[0].Replace("Question: ", "");
            var correctAnswerText = lines[1].Replace("Correct answer: ", "");
            var selectedAnswerText = lines[2].Replace("Selected answer: ", "");

            var requestData = new
            {
                model = "phi3:latest",
                prompt = $"As a computer science tutor, explain in 1-2 sentences why '{correctAnswerText}' is correct " +
                        $"and why '{selectedAnswerText}' is incorrect for: '{questionText}'. " +
                        "Then suggest one study topic.",
                stream = false,
                options = new
                {
                    temperature = 0.7,
                    num_ctx = 2048
                }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);
            return result?.response ?? "No explanation generated.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI call failed: {ex}");
            return "Couldn't generate explanation (service error)";
        }
    }

    private string GenerateSimpleExplanation(Question question, Answer correctAnswer)
    {
        // Fallback simple explanation generator
        return $"it directly relates to the question about {question.Text.Split(' ')[0]}.";
    }

    private class AIResponse
    {
        public string answer { get; set; }
        public float score { get; set; }
    }
}