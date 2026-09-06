using System;

namespace Ciao.RC
{
    /// <summary> Root persisted object at Application.persistentDataPath. </summary>
    [Serializable]
    public class RewardCenterState
    {
        public Campaign activeCampaign;
        public bool hasShownWelcome;
        public bool hasShownOnboarding;
    }
}