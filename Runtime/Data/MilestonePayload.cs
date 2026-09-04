using System;

namespace CiaoGames.RewardCenter
{
    /// <summary> Wire-format representation of a milestone as delivered via deep link or config JSON. </summary>
    [Serializable]
    public class MilestonePayload
    {
        /// <summary>Event name (matches Singular event name).</summary>
        public string e;
        /// <summary>Event description.</summary>
        public string d;
        /// <summary>Reward amount in campaign currency.</summary>
        public float r;
    }
}