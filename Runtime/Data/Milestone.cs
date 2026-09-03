using System;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// A single offer step within a campaign. Completed when a matching event
    /// is reported with value = <see cref="targetValue"/>.
    /// </summary>
    [Serializable]
    public class Milestone
    {
        public string eventName;
        public string description;
        public int targetValue;
        public float rewardAmount;
        public bool isCompleted;
        public string completedAt;
    }
}