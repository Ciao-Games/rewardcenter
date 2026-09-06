using System;

namespace Ciao.RC
{
    /// <summary> A single offer step within a campaign.  </summary>
    [Serializable]
    public class Milestone
    {
        public string eventName;
        public string description;
        public float rewardAmount;
        public bool isCompleted;
        public string completedAt;
    }
}