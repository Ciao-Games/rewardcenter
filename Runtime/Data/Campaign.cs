using System;
using System.Collections.Generic;

namespace CiaoGames.RewardCenter
{
    /// <summary> An active offerwall campaign. Parsed from a deep link, persisted locally. </summary>
    [Serializable]
    public class Campaign
    {
        public string id;
        public string publisherName;
        public string publisherLogoUrl;
        public string currencyName;
        public string currencyIconUrl;
        public string expiresAt;
        public bool   isCompleted;
        public List<Milestone> milestones = new List<Milestone>();
    }
}