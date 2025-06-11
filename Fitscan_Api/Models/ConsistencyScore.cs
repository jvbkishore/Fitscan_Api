
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Numerics;
using Fitscan_Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitscan_Api.Models
{
    public class ConsistencyScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Username { get; set; } // Unique per user

        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double WeeklyAverage { get; set; }
        public int MonthlyVisits { get; set; }
        public DateTime LastVisitDate { get; set; }
        public int ImprovementPercent { get; set; }
        public int ReferralsThisMonth { get; set; }
        public bool TriedNewClass { get; set; }
        public bool JoinedChallenge { get; set; }

        // Auto-calculated fields
        [NotMapped]
        public int Score => CalculateScore();

        [NotMapped]
        public string MotivationalMessage => GetMessage(Score);

        private int CalculateScore()
        {
            int score = 0;
            score += Math.Min(CurrentStreak, 20);
            score += LongestStreak >= 15 ? 10 : (int)(LongestStreak * 0.66);
            score += WeeklyAverage >= 4 ? 20 : (int)(WeeklyAverage * 5);
            score += MonthlyVisits >= 16 ? 20 : (int)(MonthlyVisits * 1.25);

            var daysSinceLastVisit = (DateTime.Now.Date - LastVisitDate.Date).Days;
            if (daysSinceLastVisit <= 1) score += 10;
            else if (daysSinceLastVisit <= 3) score += 5;

            score += Math.Min(ImprovementPercent, 10);

            int bonus = 0;
            if (ReferralsThisMonth > 0) bonus += Math.Min(ReferralsThisMonth * 3, 6);
            if (TriedNewClass) bonus += 2;
            if (JoinedChallenge) bonus += 2;

            score += Math.Min(bonus, 10);
            return Math.Min(score, 100);
        }

        private string GetMessage(int score)
        {
            return score switch
            {
                >= 90 => "You're a fitness machine! Keep dominating!",
                >= 75 => "Great consistency! You're crushing your goals!",
                >= 50 => "Good progress! Keep building momentum!",
                _ => "Let's get moving! Every workout counts!"
            };
        }
    }
}

//| Factor | Description | Max Points | Formula / Rule |
//| -------------------- | -------------------------------------- | ---------- | -------------------------------------------------- |
//| **Current Streak * *   | Continuous gym visits without a break  | 20         | `min(currentStreak, 20)`                           |
//| **Longest Streak**   | User's personal best streak            | 10         | `longestStreak >= 15 ? 10 : longestStreak * 0.66`  |
//| **Weekly Avg**       | Avg visits/week (last 4 weeks)         | 20         | `weeklyAverage >= 4 ? 20 : weeklyAverage * 5`      |
//| **Monthly Visits * *   | Total visits in current month          | 20         | `monthlyVisits >= 16 ? 20 : monthlyVisits * 1.25`  |
//| **Last Visit * *       | Recency of visit                       | 10         | `yesterday = 10, within 3 days = 5, otherwise = 0` |
//| **Improvement**      | % improvement over last month          | 10         | `min(improvement, 10)`                             |
//| **Bonus/Engagement** | Referrals, new class, challenge joined | 10         | Based on events triggered                          |




//consistencyScore = currentStreakScore
//                 + longestStreakScore
//                 + weeklyAvgScore
//                 + monthlyVisitsScore
//                 + lastVisitScore
//                 + improvementScore
//                 + bonusScore;

