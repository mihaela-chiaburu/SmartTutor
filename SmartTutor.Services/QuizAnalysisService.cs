using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTuror.Services
{
    public class QuizAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const double FAST_RESPONSE_THRESHOLD = 15.0; // seconds
        private const double SLOW_RESPONSE_THRESHOLD = 45.0; // seconds
        private const int MAX_TAB_SWITCHES = 3;
        private const double CONFIDENCE_THRESHOLD = 0.7;

        public QuizAnalysisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DifficultyLevel> AnalyzeAndAdjustDifficulty(
            string userId,
            int quizId,
            int currentQuestionId,
            RealTimePerformance performance)
        {
            // Get recent performance data for this user and quiz
            var recentPerformances = await _unitOfWork.RealTimePerformances.GetAllAsync(
                p => p.UserId == userId && p.QuizId == quizId
            );

            // Order and take the most recent 5 performances
            recentPerformances = recentPerformances
                .OrderByDescending(p => p.Timestamp)
                .Take(5)
                .ToList();

            // Calculate performance metrics
            var averageResponseTime = recentPerformances.Average(p => p.ResponseTime);
            var averageTabSwitches = recentPerformances.Average(p => p.TabSwitches);
            var correctAnswersCount = recentPerformances.Count(p => p.IsCorrect);
            var averageConfidence = recentPerformances.Average(p => p.ConfidenceLevel);

            // Get current question
            var currentQuestion = await _unitOfWork.Question.GetAsync(q => q.Id == currentQuestionId);
            if (currentQuestion == null) return DifficultyLevel.Medium;

            // Determine if we should adjust difficulty
            bool shouldIncreaseDifficulty =
                averageResponseTime < FAST_RESPONSE_THRESHOLD &&
                averageTabSwitches < MAX_TAB_SWITCHES &&
                correctAnswersCount >= 3 &&
                averageConfidence >= CONFIDENCE_THRESHOLD;

            bool shouldDecreaseDifficulty =
                averageResponseTime > SLOW_RESPONSE_THRESHOLD ||
                averageTabSwitches >= MAX_TAB_SWITCHES ||
                correctAnswersCount <= 1 ||
                averageConfidence < CONFIDENCE_THRESHOLD;

            // Adjust difficulty
            if (shouldIncreaseDifficulty && currentQuestion.Difficulty < DifficultyLevel.Hard)
            {
                currentQuestion.Difficulty = (DifficultyLevel)((int)currentQuestion.Difficulty + 1);
                await _unitOfWork.SaveAsync();
            }
            else if (shouldDecreaseDifficulty && currentQuestion.Difficulty > DifficultyLevel.Easy)
            {
                currentQuestion.Difficulty = (DifficultyLevel)((int)currentQuestion.Difficulty - 1);
                await _unitOfWork.SaveAsync();
            }

            return currentQuestion.Difficulty;
        }

        public async Task<double> CalculateConfidenceLevel(
            double responseTime,
            int tabSwitches,
            bool isCorrect)
        {
            // Normalize response time (0-1 scale, where 1 is fastest)
            double normalizedResponseTime = Math.Max(0, 1 - (responseTime / SLOW_RESPONSE_THRESHOLD));

            // Normalize tab switches (0-1 scale, where 1 is no tab switches)
            double normalizedTabSwitches = Math.Max(0, 1 - (tabSwitches / (double)MAX_TAB_SWITCHES));

            // Calculate confidence based on multiple factors
            double confidence = (
                (normalizedResponseTime * 0.4) + // 40% weight for response time
                (normalizedTabSwitches * 0.2) +  // 20% weight for tab switches
                (isCorrect ? 0.4 : 0.1)          // 40% weight for correctness
            );

            return Math.Min(1.0, Math.Max(0.0, confidence));
        }
    }
}