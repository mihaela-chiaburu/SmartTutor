using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
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

        public async Task<IActionResult> CreateQuiz(int chapterId)
        {
            var chapter = await _unitOfWork.Chapter.GetAsync(c => c.ChapterId == chapterId, includeProperties: "Course");
            if (chapter == null)
            {
                TempData["error"] = "Chapter not found";
                return NotFound();
            }
            var quiz = new Quiz
            {
                ChapterId = chapterId,
                Title = $"{chapter.Course?.Title ?? "Unknown Course"} - Chapter {chapter.ChapterId} Quiz",
                CreatedDate = DateTime.Now // Add timestamp
            };

            try
            {
                _unitOfWork.Quiz.Add(quiz);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Quiz created successfully";
                return RedirectToAction("ManageQuestions", new { chapterId });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error creating quiz: {ex.Message}";
                return RedirectToAction("Index", "Home"); // Or appropriate error handling
            }
        }

        public async Task<IActionResult> ManageQuestions(int chapterId)
        {
            var quiz = await _unitOfWork.Quiz.GetAsync(q => q.ChapterId == chapterId, includeProperties: "Questions,Questions.Answers,Chapter");
            if (quiz == null) return RedirectToAction("CreateQuiz", new { chapterId });

            var vm = new QuestionManagementVM
            {
                ChapterId = chapterId,
                ChapterTitle = quiz.Chapter.Title,
                Questions = quiz.Questions?.ToList() ?? new List<Question>()
            };

            return View(vm);
        }

        public async Task<IActionResult> CreateQuestion(int chapterId)
        {
            var quiz = await _unitOfWork.Quiz.GetAsync(q => q.ChapterId == chapterId);
            if (quiz == null) return NotFound();

            var vm = new QuestionVM
            {
                Question = new Question
                {
                    QuizId = quiz.Id,
                    Answers = new List<Answer> { new Answer() } // Start with one empty answer
                },
                ChapterId = chapterId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Debug output
            Console.WriteLine($"Creating question for QuizId: {vm.Question.QuizId}");
            Console.WriteLine($"Question text: {vm.Question.Text}");
            Console.WriteLine($"Difficulty: {vm.Question.Difficulty}");

            foreach (var answer in vm.Question.Answers)
            {
                Console.WriteLine($"Answer: {answer.Text}, Correct: {answer.IsCorrect}");
            }

            try
            {
                // Explicitly add question and answers
                _unitOfWork.Question.Add(vm.Question);

                foreach (var answer in vm.Question.Answers)
                {
                    answer.QuestionId = vm.Question.Id; // Ensure relationship
                    _unitOfWork.Answer.Add(answer);
                }

                await _unitOfWork.SaveAsync();

                Console.WriteLine($"Successfully saved question with ID: {vm.Question.Id}");
                TempData["success"] = "Question created successfully";
                return RedirectToAction("ManageQuestions", new { chapterId = vm.ChapterId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving question: {ex}");
                TempData["error"] = $"Error creating question: {ex.Message}";
                return View(vm);
            }
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

                    bool isCorrect = selectedAnswer != null && selectedAnswer.IsCorrect == true;

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
        public IActionResult SaveUserAnswer(int quizId, int questionId, int answerId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();

            // Find the correct answer
            var correctAnswer = _unitOfWork.Answer
                .Get(a => a.QuestionId == questionId && a.IsCorrect == true);

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

            // Return feedback data
            return Json(new
            {
                success = true,
                isCorrect,
                correctAnswerId = correctAnswer?.Id
            });
        }

    }
}
