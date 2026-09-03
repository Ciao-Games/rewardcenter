using UnityEngine;

namespace CiaoGames.RewardCenter
{
    [CreateAssetMenu(fileName = nameof(RewardCenterConfig), menuName = "Ciao Games/Reward Center Config")]
    public class RewardCenterConfig : ScriptableObject
    {
        [Header("Logging")]
        public LogLevel logLevel = LogLevel.Warn;
        
        [Header("Behavior")]
        [Tooltip("If true, keep showing the tracker card even after the user completes all milestones.")]
        public bool showTrackerAfterCompletion = false;
    }
}