using System.Collections.Generic;

namespace CiaoGames.RewardCenter
{
    /// <summary> Converts wire-format payloads (short-key DTOs) into runtime models. </summary>
    public static class PayloadMapper
    {
        private const string ClassName = nameof(PayloadMapper);
        public static Campaign ToCampaign(CampaignPayload payload)
        {
            if (!IsPayloadValid(payload))
                return null;
            
            Campaign campaign = GetMainCampaignData(payload);
            foreach (MilestonePayload milestone in payload.m)
                campaign.milestones.Add(ToMilestone(milestone));

            return campaign;
        }

        private static bool IsPayloadValid(CampaignPayload payload)
        {
            if (payload == null) return false;
            if (payload.m == null || payload.m.Count == 0)
            {
                RewardCenterLogger.LogWarning(ClassName,"Campaign payload has no milestones, skipping.");
                return false;
            }
            return true;
        }

        private static Campaign GetMainCampaignData(CampaignPayload payload)
        {
            return new Campaign
            {
                id = payload.id,
                publisherName = payload.pn,
                publisherLogoUrl = payload.plu,
                currencyName = payload.cn,
                currencyIconUrl = payload.ciu,
                expiresAt = payload.ex
            };
        }

        private static Milestone ToMilestone(MilestonePayload payload)
        {
            if (payload == null) return null;
            return new Milestone
            {
                eventName = payload.e,
                description = payload.d,
                targetValue = payload.t,
                rewardAmount = payload.r
            };
        }
    }
}