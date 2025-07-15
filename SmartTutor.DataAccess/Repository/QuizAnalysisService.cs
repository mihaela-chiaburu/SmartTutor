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
        private const double FAST_RESPONSE_THRESHOLD = 15.0; 
        private const double SLOW_RESPONSE_THRESHOLD = 45.0; 
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
            var recentPerformances = await _unitOfWork.RealTimePerformances.GetAllAsync(
                p => p.UserId == userId && p.QuizId == quizId
            );

            recentPerformances = recentPerformances
                .OrderByDescending(p => p.Timestamp)
                .Take(5)
                .ToList();

            var averageResponseTime = recentPerformances.Average(p => p.ResponseTime);
            var averageTabSwitches = recentPerformances.Average(p => p.TabSwitches);
            var correctAnswersCount = recentPerformances.Count(p => p.IsCorrect);
            var averageConfidence = recentPerformances.Average(p => p.ConfidenceLevel);

            var currentQuestion = await _unitOfWork.Question.GetAsync(q => q.Id == currentQuestionId);
            if (currentQuestion == null) return DifficultyLevel.Medium;

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
            double normalizedResponseTime = Math.Max(0, 1 - (responseTime / SLOW_RESPONSE_THRESHOLD));

            double normalizedTabSwitches = Math.Max(0, 1 - (tabSwitches / (double)MAX_TAB_SWITCHES));

            double confidence = (
                (normalizedResponseTime * 0.4) + 
                (normalizedTabSwitches * 0.2) +  
                (isCorrect ? 0.4 : 0.1)          
            );

            return Math.Min(1.0, Math.Max(0.0, confidence));
        }
    }
}