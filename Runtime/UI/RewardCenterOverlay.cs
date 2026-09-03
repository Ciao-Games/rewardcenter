using UnityEngine;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// Coordinator for the RewardCenterOverlay prefab. Manages the full-screen dimmer,
    /// WelcomePopup, and MilestonesMapPopup. Discovers the studio's TrackerCard at runtime
    /// (even if inactive) and drives its visibility, progress, and spotlight.
    /// </summary>
    public class RewardCenterOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject overlay;
        [SerializeField] private WelcomePopup welcomePopup;
        [SerializeField] private MilestonesMapPopup milestonesMapPopup;
        [SerializeField] private CelebrationPopupBase celebrationPopup;

        private TrackerCard _trackerCard;
        private Sprite _publisherLogoSprite;
        private Sprite _currencyIconSprite;

        private void Start()
        {
            if (welcomePopup != null) welcomePopup.OnContinueClicked += OnWelcomeContinueClick;
            if (milestonesMapPopup != null) milestonesMapPopup.OnContinueClicked += OnMilestonesMapContinueClick;
            if (celebrationPopup != null) celebrationPopup.OnContinueClicked += OnCelebrationContinueClick;

            RewardCenter.OnCampaignActivated += OnCampaignActivated;
            RewardCenter.OnMilestoneCompleted += OnMilestoneCompleted;

            DiscoverTracker();
            LoadSprites();
            RefreshFromState();
        }

        private void OnDestroy()
        {
            if (welcomePopup != null) welcomePopup.OnContinueClicked -= OnWelcomeContinueClick;
            if (milestonesMapPopup != null) milestonesMapPopup.OnContinueClicked -= OnMilestonesMapContinueClick;
            if (_trackerCard != null) _trackerCard.OnViewClicked -= OpenMilestonesMap;
            if (celebrationPopup != null) celebrationPopup.OnContinueClicked -= OnCelebrationContinueClick;

            RewardCenter.OnCampaignActivated -= OnCampaignActivated;
            RewardCenter.OnMilestoneCompleted -= OnMilestoneCompleted;
        }

        private void DiscoverTracker()
        {
            if (_trackerCard != null) return;
            _trackerCard = FindFirstObjectByType<TrackerCard>(FindObjectsInactive.Include);
            if (_trackerCard != null) _trackerCard.OnViewClicked += OpenMilestonesMap;
        }

        private void RefreshFromState()
        {
            var campaign = RewardCenter.State?.activeCampaign;
            if (campaign == null)
            {
                HideAll();
                return;
            }

            var showWelcome = !RewardCenter.State.hasShownWelcome;
            var showTracker = !showWelcome && ShouldShowTracker(campaign);
            var showOnboarding = showTracker && !RewardCenter.State.hasShownOnboarding;

            if (welcomePopup != null)
            {
                if (showWelcome) welcomePopup.Show(_publisherLogoSprite);
                else welcomePopup.Hide();
            }

            if (_trackerCard != null)
            {
                if (showTracker)
                    _trackerCard.Show(_currencyIconSprite, RewardCenter.EarnedRewardAmount, RewardCenter.TotalRewardAmount);                else
                    _trackerCard.Hide();

                if (showOnboarding) _trackerCard.EnableSpotlight();
                else _trackerCard.DisableSpotlight();
            }

            if (milestonesMapPopup != null) milestonesMapPopup.Hide();

            UpdateOverlay(showOnboarding);
        }

        // ----------- CHILD EVENT HANDLERS -----------

        private void OnWelcomeContinueClick()
        {
            RewardCenter.MarkWelcomeShown();
            RefreshFromState();
        }

        private void OnMilestonesMapContinueClick()
        {
            if (milestonesMapPopup != null) milestonesMapPopup.Hide();
            UpdateOverlay(false);
        }

        public void OpenMilestonesMap()
        {
            var campaign = RewardCenter.State?.activeCampaign;
            if (campaign == null || milestonesMapPopup == null) return;

            RewardCenter.MarkOnboardingShown();
            if (_trackerCard != null) _trackerCard.DisableSpotlight();

            milestonesMapPopup.Show(campaign, _publisherLogoSprite, _currencyIconSprite, RewardCenter.CompletedMilestoneCount, RewardCenter.TotalMilestoneCount);
            UpdateOverlay(true);
        }

        // ----------- FACADE EVENT HANDLERS -----------

        private void OnCampaignActivated(Campaign campaign)
        {
            LoadSprites();
            DiscoverTracker();
            RefreshFromState();
        }

        private void OnMilestoneCompleted(Milestone milestone)
        {
            var campaign = RewardCenter.State?.activeCampaign;
            if (campaign == null || celebrationPopup == null) return;

            if (_trackerCard != null) _trackerCard.DisableSpotlight();

            celebrationPopup.Show(milestone, campaign.currencyName, _currencyIconSprite);
            UpdateOverlay(true);
        }
        
        private void OnCelebrationContinueClick()
        {
            if (celebrationPopup != null) celebrationPopup.Hide();
            if (_trackerCard != null) _trackerCard.SetProgress(RewardCenter.EarnedRewardAmount, RewardCenter.TotalRewardAmount);
            UpdateOverlay(false);
        }

        // ----------- HELPERS -----------

        private static bool ShouldShowTracker(Campaign campaign)
        {
            if (campaign.isCompleted && RewardCenter.Config != null && !RewardCenter.Config.showTrackerAfterCompletion)
                return false;
            return true;
        }

        private void LoadSprites()
        {
            _publisherLogoSprite = RewardCenterUtils.LoadSpriteFromDisk(AssetPaths.PublisherLogo);
            _currencyIconSprite = RewardCenterUtils.LoadSpriteFromDisk(AssetPaths.CurrencyIcon);
        }

        private void HideAll()
        {
            if (welcomePopup != null) welcomePopup.Hide();
            if (milestonesMapPopup != null) milestonesMapPopup.Hide();
            if (celebrationPopup != null) celebrationPopup.Hide();
            if (_trackerCard != null)
            {
                _trackerCard.Hide();
                _trackerCard.DisableSpotlight();
            }
            if (overlay != null) overlay.SetActive(false);
        }

        private void UpdateOverlay(bool onboardingActive)
        {
            if (overlay == null) return;
            bool welcomeActive = welcomePopup != null && welcomePopup.gameObject.activeSelf;
            bool mapActive = milestonesMapPopup != null && milestonesMapPopup.gameObject.activeSelf;
            bool celebrationActive = celebrationPopup != null && celebrationPopup.gameObject.activeSelf;
            overlay.SetActive(welcomeActive || mapActive || celebrationActive || onboardingActive);
        }
    }
}