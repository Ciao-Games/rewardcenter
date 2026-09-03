using System;
using System.Collections.Generic;
using System.Linq;

namespace CiaoGames.RewardCenter
{
    public class EventRouter
    {
        private readonly StateStore _stateStore;
        /// <summary> Raised when a Milestone completed by a ReportEvent call</summary>
        public event Action<Milestone> OnMilestoneCompleted;

        public EventRouter(StateStore stateStore)
        {
            _stateStore = stateStore;
        }
        
        public void ReportEvent(string eventName, int targetValue)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            var campaign = _stateStore.State?.activeCampaign;
            if (campaign == null) return;
            Milestone completedMilestone = null;

            foreach (var milestone in campaign.milestones)
            {
                if (milestone.isCompleted) continue;
                if (milestone.eventName != eventName) continue;
                if (targetValue != -1 && milestone.targetValue != targetValue) continue;
                
                SetMilestoneCompleted(milestone);
                completedMilestone = milestone;
                break;
            }
            if (completedMilestone == null) return;

            if (campaign.milestones.All(m => m.isCompleted))
            {
                campaign.isCompleted = true;
            }
            _stateStore.Save();
            OnMilestoneCompleted?.Invoke(completedMilestone);
        }

        private void SetMilestoneCompleted(Milestone milestone)
        {
            milestone.isCompleted = true;
            milestone.completedAt = DateTime.UtcNow.ToString("o");
        }
    }
}