using Lib.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTutor.Models;
using SmartTutor.Models.ViewModels;
using System.Text.Json;

namespace SmartTutor.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class QuizController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AIService _aiService;
        private readonly UserManager<IdentityUser> _userManager;

        public QuizController(IUnitOfWork unitOfWork,
                            AIService aiService,
                            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _aiService = aiService;
            _userManager = userManager;
        }

        public IActionResult Quiz()
        {
            return View();
        }

        public IActionResult TakeQuiz(int chapterId)
        {
            var quiz = _unitOfWork.Quiz.Get(
                q => q.ChapterId == chapterId,
                includeProperties: "Questions,Questions.Answers"
            );

            if (quiz == null)
            {
                return NotFound();
            }

            var viewModel = new QuizViewModel
            {
                Quiz = quiz,
                ChapterId = chapterId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(QuizSubmission submission)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _unitOfWork.Quiz.GetAsync(
                q => q.Id == submission.QuizId,
                includeProperties: "Questions,Questions.Answers,Chapter");

            if (quiz == null) return NotFound();

            // Calculate score
            var correctAnswers = quiz.Questions.Count(q =>
                submission.Answers.Any(a =>
                    a.QuestionId == q.Id &&
                    q.Answers.Any(ca => ca.Id == a.AnswerId && ca.IsCorrect)));

            var score = (double)correctAnswers / quiz.Questions.Count * 100;

            // Perform AI analysis
            var analysisRequest = new QuizAnalysisRequest
            {
                UserId = user.Id,
                ChapterId = quiz.ChapterId,
                Answers = submission.Answers,
                TimeTaken = submission.TimeTaken,
                TabSwitches = submission.TabSwitches
            };

            var analysis = await _aiService.AnalyzeQuizPerformance(analysisRequest);

            // Save result
            var result = new QuizResult
            {
                UserId = user.Id,
                QuizId = quiz.Id,
                Score = score,
                TimeTaken = submission.TimeTaken,
                TabSwitches = submission.TabSwitches,
                ConfidenceLevel = analysis.ConfidenceLevel,
                SuggestedResources = JsonSerializer.Serialize(analysis.SuggestedResources),
                TakenOn = DateTime.Now
            };

            _unitOfWork.QuizResult.Add(result);
            await _unitOfWork.SaveAsync();

            // Return results
            return View("QuizResult", new QuizResultViewModel
            {
                Score = score,
                Analysis = analysis,
                Chapter = quiz.Chapter
            });
        }
    }
}
