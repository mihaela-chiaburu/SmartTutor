using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using SmartTuror.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTutor.Models;
using SmartTutor.Models.ViewModels;
using System.Text.Json;

namespace SmartTutor.Areas.Student.Controllers
{
    [Area("Student")]
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
                includeProperties: "Questions,Questions.Answers,Chapter"
            );

            if (quiz == null)
            {
                return NotFound();
            }

            // Get the first question, prioritizing easy questions
            var firstQuestion = quiz.Questions
                .FirstOrDefault(q => q.Difficulty.ToString() == "Easy") 
                ?? quiz.Questions.FirstOrDefault();

            var viewModel = new QuizViewModel
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                ChapterId = chapterId,
                CurrentQuestion = firstQuestion != null ? new QuestionViewModel
                {
                    Id = firstQuestion.Id,
                    Text = firstQuestion.Text,
                    Difficulty = firstQuestion.Difficulty.ToString(),
                    Answers = firstQuestion.Answers.Select(a => new AnswerViewModel
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                } : null
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
        public async Task<IActionResult> SaveUserAnswer(int quizId, int questionId, int answerId, double responseTime, int tabSwitches)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var question = await _unitOfWork.Question.GetAsync(q => q.Id == questionId, includeProperties: "Answers");
                if (question == null)
                {
                    return Json(new { success = false, message = "Question not found" });
                }

                var isCorrect = question.Answers.FirstOrDefault(a => a.Id == answerId)?.IsCorrect ?? false;

                // Get user's recent answers for this quiz
                var recentAnswers = await _unitOfWork.UserAnswer.GetAllAsync(
                    ua => ua.QuizId == quizId && ua.UserId == user.Id
                );
                recentAnswers = recentAnswers.OrderByDescending(x => x.AnsweredOn).Take(5).ToList();

                // Calculate accuracy from recent answers
                var accuracy = recentAnswers.Any() 
                    ? (double)recentAnswers.Count(a => a.IsCorrect) / recentAnswers.Count() 
                    : 0;

                // Determine next question difficulty based on performance
                DifficultyLevel nextDifficulty;
                if (recentAnswers.Count() < 10) // Only adapt difficulty if we haven't reached 10 questions
                {
                    if (isCorrect)
                    {
                        if (responseTime < 30 && tabSwitches == 0) // Fast and focused
                        {
                            nextDifficulty = question.Difficulty == DifficultyLevel.Easy ? DifficultyLevel.Medium : 
                                            question.Difficulty == DifficultyLevel.Medium ? DifficultyLevel.Hard : 
                                            DifficultyLevel.Hard;
                        }
                        else // Slow or unfocused
                        {
                            nextDifficulty = DifficultyLevel.Easy;
                        }
                    }
                    else // Incorrect answer
                    {
                        nextDifficulty = question.Difficulty == DifficultyLevel.Hard ? DifficultyLevel.Medium : 
                                        DifficultyLevel.Easy;
                    }
                }
                else
                {
                    nextDifficulty = (DifficultyLevel)(-1); // No more questions needed
                }

                // Get next question if needed
                Question nextQuestion = null;
                if (nextDifficulty != (DifficultyLevel)(-1))
                {
                    // Get all questions for this quiz that haven't been answered yet
                    var answeredQuestionIds = recentAnswers.Select(a => a.QuestionId).ToList();
                    answeredQuestionIds.Add(questionId); // Include current question

                    nextQuestion = await _unitOfWork.Question.GetAsync(
                        q => q.QuizId == quizId && 
                             q.Difficulty == nextDifficulty && 
                             !answeredQuestionIds.Contains(q.Id),
                        includeProperties: "Answers"  // Make sure to include answers
                    );

                    // If no questions of desired difficulty, try other difficulties
                    if (nextQuestion == null)
                    {
                        nextQuestion = await _unitOfWork.Question.GetAsync(
                            q => q.QuizId == quizId && 
                                 !answeredQuestionIds.Contains(q.Id),
                            includeProperties: "Answers"  // Make sure to include answers
                        );
                    }
                }

                // Save the user's answer
                var userAnswer = new UserAnswer
                {
                    UserId = user.Id,
                    QuizId = quizId,
                    QuestionId = questionId,
                    AnswerId = answerId,
                    IsCorrect = isCorrect,
                    AnsweredOn = DateTime.Now,
                    ResponseTime = responseTime,
                    TabSwitches = tabSwitches
                };

                _unitOfWork.UserAnswer.Add(userAnswer);
                await _unitOfWork.SaveAsync();

                // Generate explanation for incorrect answers
                string explanation;
                try
                {
                    explanation = isCorrect 
                        ? "Correct! Well done!" 
                        : await _aiService.GenerateExplanation(questionId, answerId);
                }
                catch (Exception ex)
                {
                    // If AI service fails, use a default explanation
                    explanation = isCorrect 
                        ? "Correct! Well done!" 
                        : "Incorrect. Please review the material and try again.";
                }

                var response = new
                {
                    success = true,
                    isCorrect = isCorrect,
                    explanation = explanation,
                    nextQuestion = nextQuestion != null ? new QuestionViewModel
                    {
                        Id = nextQuestion.Id,
                        Text = nextQuestion.Text,
                        Difficulty = nextQuestion.Difficulty.ToString(),
                        Answers = nextQuestion.Answers?.Select(a => new AnswerViewModel
                        {
                            Id = a.Id,
                            Text = a.Text
                        }).ToList() ?? new List<AnswerViewModel>()
                    } : null
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveUserAnswer: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = "An error occurred while saving your answer",
                    error = ex.Message 
                });
            }
        }

    }
}
