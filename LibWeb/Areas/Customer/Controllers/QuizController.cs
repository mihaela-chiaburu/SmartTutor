using Lib.DataAccess.Repository.IRepository;
using Lib.Models.ViewModels;
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

        /*public IActionResult Quiz()
        {
            return View();
        }*/

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
        public async Task<IActionResult> SubmitQuiz(int quizId, Dictionary<int, int> SelectedAnswers, int timeTaken, int tabSwitches)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _unitOfWork.Quiz.GetAsync(
                q => q.Id == quizId,
                includeProperties: "Questions,Questions.Answers,Chapter");

            if (quiz == null) return NotFound();

            // Calculate score
            var correctAnswers = 0;
            var userAnswers = new List<UserAnswer>();

            foreach (var question in quiz.Questions)
            {
                if (SelectedAnswers.TryGetValue(question.Id, out int selectedAnswerId))
                {
                    var selectedAnswer = await _unitOfWork.Answer.GetAsync(
                        a => a.Id == selectedAnswerId && a.QuestionId == question.Id);

                    bool isCorrect = selectedAnswer != null && selectedAnswer.IsCorrect;

                    if (isCorrect) correctAnswers++;

                    userAnswers.Add(new UserAnswer
                    {
                        UserId = user.Id,
                        QuizId = quiz.Id,
                        QuestionId = question.Id,
                        AnswerId = selectedAnswerId,
                        IsCorrect = isCorrect,
                        AnsweredOn = DateTime.Now
                    });
                }
            }

            var score = (double)correctAnswers / quiz.Questions.Count * 100;

            // Save all user answers
            foreach (var answer in userAnswers)
            {
                _unitOfWork.UserAnswer.Add(answer);
            }

            // Save quiz result with default suggested resources
            var result = new QuizResult
            {
                UserId = user.Id,
                QuizId = quiz.Id,
                Score = score,
                TimeTaken = timeTaken,
                TabSwitches = tabSwitches,
                ConfidenceLevel = score / 100, // Simple confidence calculation
                //SuggestedResources = JsonSerializer.Serialize(new List<SuggestedResource>()),
                TakenOn = DateTime.Now
            };

            _unitOfWork.QuizResult.Add(result);
            await _unitOfWork.SaveAsync();

            return View("QuizResult", new QuizResultViewModel
            {
                Score = score,
                Chapter = quiz.Chapter
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAnswer(int quizId, int questionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userAnswer = await _unitOfWork.UserAnswer.GetAsync(
                ua => ua.UserId == user.Id && ua.QuizId == quizId && ua.QuestionId == questionId);

            if (userAnswer == null) return Json(new { answerId = 0 });

            return Json(new { answerId = userAnswer.AnswerId });
        }

        [HttpPost]
        [HttpPost]
        public IActionResult SaveUserAnswer(int quizId, int questionId, int answerId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();

            // Find the correct answer
            var correctAnswer = _unitOfWork.Answer
                .Get(a => a.QuestionId == questionId && a.IsCorrect);

            // Check if selected answer is correct
            bool isCorrect = correctAnswer != null && correctAnswer.Id == answerId;

            // Check if the user has already submitted an answer for this question
            var existingAnswer = _unitOfWork.UserAnswer
                .Get(ua => ua.UserId == user.Id && ua.QuestionId == questionId);

            if (existingAnswer == null)
            {
                var userAnswer = new UserAnswer
                {
                    UserId = user.Id,
                    QuizId = quizId,
                    QuestionId = questionId,
                    AnswerId = answerId,
                    IsCorrect = isCorrect,
                    AnsweredOn = DateTime.UtcNow
                };

                _unitOfWork.UserAnswer.Add(userAnswer);
            }
            else
            {
                existingAnswer.AnswerId = answerId;
                existingAnswer.IsCorrect = isCorrect;
                existingAnswer.AnsweredOn = DateTime.UtcNow;
                _unitOfWork.UserAnswer.Update(existingAnswer);
            }

            _unitOfWork.Save();
            return Json(new { success = true, isCorrect });
        }


    }
}
