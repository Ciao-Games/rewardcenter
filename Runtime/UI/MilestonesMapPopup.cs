using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// The full milestones map popup. Opens when the tracker is tapped, or automatically
    /// </summary>
    public class MilestonesMapPopup : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Image publisherLogo;
        [SerializeField] private Button continueButton;

        [Header("Summary Banner Bindings")]
        [SerializeField] private GameObject daysRemaining;
        [SerializeField] private TMP_Text daysRemainingText;
        [SerializeField] private TMP_Text milestonesCountText;
        [SerializeField] private TMP_Text rewardsEarnedText;
        [SerializeField] private Color completedColor;
        
        [Header("Milestones List")]
        [SerializeField] private Transform milestonesListParent;
        [SerializeField] private GameObject milestoneRowPrefab;
        
        public event Action OnContinueClicked;
        
        private string _builtForCampaignId;
        private readonly List<MilestoneRow> _rows = new List<MilestoneRow>();



        void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
        }

        public void Show(Campaign campaign, Sprite publisherLogoSprite, Sprite currencyIconSprite, int completedMilestones, int totalMilestones)
        {
            if (publisherLogo != null)
            {
                publisherLogo.sprite = publisherLogoSprite;
                publisherLogo.preserveAspect = true;
                publisherLogo.gameObject.SetActive(publisherLogoSprite != null);
            }

            // Bind summary statistics
            if (campaign != null)
            {
                if (daysRemainingText != null)
                    daysRemainingText.text = GetDaysRemainingText(campaign);
                if(daysRemaining != null)
                    daysRemaining.SetActive(!campaign.isCompleted);
                if (milestonesCountText != null)
                {
                    milestonesCountText.text = $"Milestones:\n<color=#{ColorUtility.ToHtmlStringRGB(completedColor)}>{completedMilestones}</color> / {totalMilestones}";
                }
                if (rewardsEarnedText != null)
                {
                    rewardsEarnedText.text = $"{RewardCenter.CurrencyName} earned:\n<color=#{ColorUtility.ToHtmlStringRGB(completedColor)}>{RewardCenter.EarnedRewardAmount:0}</color> / {RewardCenter.TotalRewardAmount:0}";
                }

                PopulateRows(campaign, currencyIconSprite);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private static string GetDaysRemainingText(Campaign campaign)
        {
            if (campaign.isCompleted) return "Completed!";

            if (string.IsNullOrEmpty(campaign.expiresAt)) return string.Empty;

            if (!DateTime.TryParse(campaign.expiresAt, null, DateTimeStyles.RoundtripKind, out var expiresAt))
                return string.Empty;

            var days = (int)Math.Ceiling((expiresAt - DateTime.UtcNow).TotalDays);
            return days <= 0 ? "Expired" : days.ToString();
        }
        
        private void PopulateRows(Campaign campaign, Sprite currencyIconSprite)
        {
            if (milestonesListParent == null || milestoneRowPrefab == null || campaign?.milestones == null) return;

            // Build only once per campaign
            if (_builtForCampaignId != campaign.id)
            {
                DestroyExistingRows();
                for (int i = 0; i < campaign.milestones.Count; i++)
                {
                    var row = Instantiate(milestoneRowPrefab, milestonesListParent).GetComponent<MilestoneRow>();
                    if (row != null) _rows.Add(row);
                }
                _builtForCampaignId = campaign.id;
            }

            // Refresh state every time
            for (int i = 0; i < campaign.milestones.Count; i++)
            {
                _rows[i].Populate(campaign.milestones[i], i + 1, currencyIconSprite);
            }
        }

        private void DestroyExistingRows()
        {
            foreach (var row in _rows) if (row != null) Destroy(row.gameObject);
            _rows.Clear();
        }
    }
}

