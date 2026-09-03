using System;
using System.Collections.Generic;

namespace CiaoGames.RewardCenter
{
    /// <summary> Wire-format representation of a campaign as delivered via deep link or config JSON.</summary>
    [Serializable]
    public class CampaignPayload
    {
        /// <summary>Campaign id.</summary>
        public string id;

        /// <summary>Publisher name.</summary>
        public string pn;

        /// <summary>Publisher logo URL (remote).</summary>
        public string plu;

        /// <summary>Currency name.</summary>
        public string cn;

        /// <summary>Currency icon URL (remote).</summary>
        public string ciu;

        /// <summary>Campaign expiry, ISO 8601 UTC.</summary>
        public string ex;

        /// <summary>Milestones list.</summary>
        public List<MilestonePayload> m = new List<MilestonePayload>();
    }
}