// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartTutor.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList CategoryList { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Preferred Category")]
            public int? PreferredCategoryId { get; set; }

            [Display(Name = "Email Notifications")]
            public bool EmailNotifications { get; set; }

            [Display(Name = "Course Updates")]
            public bool CourseUpdates { get; set; }

            [Display(Name = "Quiz Results")]
            public bool QuizResults { get; set; }

            [Display(Name = "Progress Updates")]
            public bool ProgressUpdates { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            // Load categories for dropdown
            CategoryList = new SelectList(_unitOfWork.CourseCategory.GetAll(), "CourseCategoryId", "Name");

            // Load user preferences
            var preferences = await _unitOfWork.UserPreferences.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
            if (preferences != null)
            {
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    PreferredCategoryId = preferences.PreferredCategoryId,
                    EmailNotifications = preferences.EmailNotifications,
                    CourseUpdates = preferences.CourseUpdates,
                    QuizResults = preferences.QuizResults,
                    ProgressUpdates = preferences.ProgressUpdates
                };
            }
            else
            {
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update or create user preferences
            var preferences = await _unitOfWork.UserPreferences.GetFirstOrDefaultAsync(p => p.UserId == user.Id);
            if (preferences == null)
            {
                preferences = new UserPreferences
                {
                    UserId = user.Id,
                    PreferredCategoryId = Input.PreferredCategoryId,
                    EmailNotifications = Input.EmailNotifications,
                    CourseUpdates = Input.CourseUpdates,
                    QuizResults = Input.QuizResults,
                    ProgressUpdates = Input.ProgressUpdates
                };
                _unitOfWork.UserPreferences.Add(preferences);
            }
            else
            {
                preferences.PreferredCategoryId = Input.PreferredCategoryId;
                preferences.EmailNotifications = Input.EmailNotifications;
                preferences.CourseUpdates = Input.CourseUpdates;
                preferences.QuizResults = Input.QuizResults;
                preferences.ProgressUpdates = Input.ProgressUpdates;
                _unitOfWork.UserPreferences.Update(preferences);
            }

            await _unitOfWork.SaveAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
