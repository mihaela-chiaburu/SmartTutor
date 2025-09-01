using SmartTuror.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTutor.Models.ViewModels;

namespace SmartTutor.Areas.Student.Controllers
{
    [Area("Student")]
    public class ProgressController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ProgressController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var results = await _unitOfWork.QuizResult.GetAllAsync(
                q => q.UserId == user.Id,
                includeProperties: "Quiz");

            var orderedResults = results.OrderByDescending(r => r.TakenOn).ToList();

            var viewModel = new ProgressViewModel
            {
                QuizResults = orderedResults,
                AverageScore = orderedResults.Any() ? orderedResults.Average(r => r.Score) : 0,
                BestScore = orderedResults.Any() ? orderedResults.Max(r => r.Score) : 0,
                AttemptsCount = orderedResults.Count
            };

            return View(viewModel);
        }
    }
}
