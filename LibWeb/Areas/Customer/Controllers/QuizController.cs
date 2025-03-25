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
        public async Task<IActionResult> SubmitQuiz(QuizSubmission submission)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _unitOfWork.Quiz.GetAsync(
                q => q.Id == submission.QuizId,
                includeProperties: "Questions,Questions.Answers,Chapter");

            if (quiz == null) return NotFound();

            // Debug output to verify received data
            Console.WriteLine($"Received submission for quiz {submission.QuizId}");
            Console.WriteLine($"Time taken: {submission.TimeTaken} seconds");
            Console.WriteLine($"Tab switches: {submission.TabSwitches}");
            Console.WriteLine("Submitted answers:");
            foreach (var answer in submission.Answers)
            {
                Console.WriteLine($"- Question {answer.QuestionId} => Answer {answer.AnswerId}");
            }

            // Calculate score - UPDATED LOGIC
            var correctAnswers = 0;
            foreach (var userAnswer in submission.Answers)
            {
                // Get the correct answer for this question
                var correctAnswer = await _unitOfWork.Answer.GetAsync(
                    a => a.QuestionId == userAnswer.QuestionId && a.IsCorrect);

                if (correctAnswer != null && userAnswer.AnswerId == correctAnswer.Id)
                {
                    correctAnswers++;
                }
            }

            var score = (double)correctAnswers / quiz.Questions.Count * 100;

            // Debug output for scoring
            Console.WriteLine($"Correct answers: {correctAnswers}/{quiz.Questions.Count}");
            Console.WriteLine($"Calculated score: {score}%");

            // Save user answers to database
            foreach (var userAnswer in submission.Answers)
            {
                var correctAnswer = await _unitOfWork.Answer.GetAsync(
                    a => a.QuestionId == userAnswer.QuestionId && a.IsCorrect);

                var isCorrect = correctAnswer != null && userAnswer.AnswerId == correctAnswer.Id;

                var userAnswerEntity = new UserAnswer
                {
                    UserId = user.Id,
                    QuizId = quiz.Id,
                    QuestionId = userAnswer.QuestionId,
                    AnswerId = userAnswer.AnswerId,
                    IsCorrect = isCorrect,
                    AnsweredOn = DateTime.Now
                };

                _unitOfWork.UserAnswer.Add(userAnswerEntity);
            }

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
