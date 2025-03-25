using RestSharp;
using Newtonsoft.Json;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using SmartTutor.Models.ViewModels;

// Add this service to your project

public class AIService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AIService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

   /* public async Task<QuizAnalysis> AnalyzeQuizPerformance(QuizAnalysisRequest request)
    {
        // Get the chapter and questions for context
        var chapter = await _unitOfWork.Chapter.GetAsync(c => c.ChapterId == request.ChapterId);
        var questions = await _unitOfWork.Question.GetAllAsync(
            q => q.Quiz.ChapterId == request.ChapterId,
            includeProperties: "Answers");

        // 1. Calculate basic metrics
        /*var correctAnswers = questions.Count(q =>
            request.Answers.Any(a =>
                a.QuestionId == q.Id &&
                q.Answers.Any(ca => ca.Id == a.AnswerId && ca.IsCorrect)));*/

       // var accuracy = (double)correctAnswers / questions.Count();

        // 2. Calculate confidence score (0-1)
        //var confidenceScore = CalculateConfidenceScore(request, accuracy);

        // 3. Determine weak areas
        //var weakAreas = IdentifyWeakAreas(request, questions);

        // 4. Generate recommendations
        /*return new QuizAnalysis
        {
            ConfidenceLevel = confidenceScore,
            SuggestedResources = GenerateResources(weakAreas, chapter),
            Recommendations = GenerateRecommendation(confidenceScore, accuracy)
        };
    }*/

    /*private double CalculateConfidenceScore(QuizAnalysisRequest request, double accuracy)
    {
        // Factors affecting confidence:
        // - Accuracy (50% weight)
        // - Time per question (30% weight)
        // - Tab switches (20% weight)

        var timePerQuestion = request.TimeTaken / (double)request.Answers.Count;
        var idealTime = 30; // seconds per question

        // Normalize time score (closer to ideal is better)
        var timeScore = 1 - Math.Min(1, Math.Abs(timePerQuestion - idealTime) / idealTime);

        // Tab switch penalty (each switch reduces score)
        var tabSwitchScore = Math.Max(0, 1 - (request.TabSwitches * 0.1));

        return (accuracy * 0.5) + (timeScore * 0.3) + (tabSwitchScore * 0.2);
    }*/

    /*private List<string> IdentifyWeakAreas(QuizAnalysisRequest request, IEnumerable<Question> questions)
    {
        var weakAreas = new List<string>();

        // Group questions by tags or categories in a real implementation
        // For now, we'll just look at incorrect answers
        var incorrectQuestions = questions.Where(q =>
            request.Answers.Any(a =>
                a.QuestionId == q.Id &&
                !q.Answers.Any(ca => ca.Id == a.AnswerId && ca.IsCorrect)));

        // Add simple tags based on question text (simplified)
        foreach (var question in incorrectQuestions)
        {
            if (question.Text.Contains("OOP") && !weakAreas.Contains("OOP"))
                weakAreas.Add("OOP");
            if (question.Text.Contains("inheritance") && !weakAreas.Contains("inheritance"))
                weakAreas.Add("inheritance");
            // Add more patterns as needed
        }

        return weakAreas;
    }*/

    private List<LearningResource> GenerateResources(List<string> weakAreas, Chapter chapter)
    {
        var resources = new List<LearningResource>();

        // In a real app, these would come from a database or external API
        foreach (var area in weakAreas)
        {
            resources.Add(new LearningResource
            {
                Title = $"{area} Fundamentals",
                Url = $"https://example.com/learn/{area.ToLower()}",
                Description = $"Learn the basics of {area} relevant to {chapter.Title}"
            });
        }

        // Add a general resource if no specific weak areas
        if (!resources.Any())
        {
            resources.Add(new LearningResource
            {
                Title = $"General {chapter.Title} Resources",
                Url = "https://example.com/learn",
                Description = "Review materials for this chapter"
            });
        }

        return resources;
    }

    private string GenerateRecommendation(double confidenceScore, double accuracy)
    {
        if (confidenceScore > 0.8 && accuracy > 0.9)
            return "Excellent performance! You've mastered this material.";

        if (confidenceScore > 0.6)
            return "Good understanding, but review suggested resources to strengthen your knowledge.";

        if (accuracy > 0.5)
            return "You're on the right track, but need more practice with these concepts.";

        return "We recommend reviewing the foundational concepts before proceeding.";
    }
}
