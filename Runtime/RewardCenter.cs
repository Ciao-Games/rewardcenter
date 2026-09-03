
using System;
using System.Globalization;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    public static class RewardCenter
    {
        private const string ClassName = nameof(RewardCenter);
        // Accessors
        private static bool _isInitialized;
        private static RewardCenterConfig _config;
        private static StateStore _stateStore;
        private static EventRouter _eventRouter;
        private static DeepLinkHandler _deepLinkHandler;
        private static AssetDownloader _assetDownloader;
        
        // Events
        public static event Action<Campaign> OnCampaignActivated;
        public static event Action<Milestone> OnMilestoneCompleted;
        
        public static bool IsInitialized => _isInitialized;
        public static RewardCenterState State => _stateStore?.State;
        public static RewardCenterConfig Config => _config;
        public static int CompletedMilestoneCount => GetCompletedCount();
        public static int TotalMilestoneCount => State?.activeCampaign?.milestones?.Count ?? 0;


        /// <summary> Initialize RewardCenter </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;
            LoadConfiguration();
            SetLogger();
            SetStateStore();
            var campaign = _stateStore.GetCampaign();
            if (campaign != null) OnCampaignActivated?.Invoke(campaign);
            _deepLinkHandler = new DeepLinkHandler();
            _assetDownloader = new AssetDownloader();
            _eventRouter = new EventRouter(_stateStore);
            _eventRouter.OnMilestoneCompleted += m => OnMilestoneCompleted?.Invoke(m);
            _isInitialized = true;
            RewardCenterLogger.Log(ClassName, "Initialized.");
        }

        /// <summary> Handle a deep link </summary>
        /// <param name="url"></param>
        public static void HandleDeepLink(string url)
        {
            if (!InitializationCheck()) return;
            _deepLinkHandler.ParseUrl(url, OnDeepLinkParsedComplete);
        }

        /// <summary> Report an event </summary>
        /// <param name="eventName"></param> <param name="value"></param>
        public static void ReportEvent(string eventName, int value = -1)
        {
            if (!InitializationCheck()) return;
            _eventRouter.ReportEvent(eventName, value);
        }
        
        public static void MarkWelcomeShown()
        {
            if (!InitializationCheck()) return;
            _stateStore.SetWelcomeShown(true);
            _stateStore.Save();
        }

        public static void MarkOnboardingShown()
        {
            if (!InitializationCheck()) return;
            _stateStore.SetOnboardingShown(true);
            _stateStore.Save();
        }
        
        public static string CurrencyName => _stateStore?.State?.activeCampaign?.currencyName;

        public static float EarnedRewardAmount
        {
            get
            {
                var milestones = _stateStore?.State?.activeCampaign?.milestones;
                if (milestones == null) return 0f;
                var sum = 0f;
                foreach (var m in milestones) if (m.isCompleted) sum += m.rewardAmount;
                return sum;
            }
        }

        public static float TotalRewardAmount
        {
            get
            {
                var milestones = _stateStore?.State?.activeCampaign?.milestones;
                if (milestones == null) return 0f;
                var sum = 0f;
                foreach (var m in milestones) sum += m.rewardAmount;
                return sum;
            }
        }
        
        
        // ----------- PRIVATE METHODS -----------

        private static int GetCompletedCount()
        {
            var milestones = State?.activeCampaign?.milestones;
            if (milestones == null) return 0;
            var count = 0;
            foreach (var m in milestones) if (m.isCompleted) count++;
            return count;
        }
        
        private static void LoadConfiguration()
        {
            _config = Resources.Load<RewardCenterConfig>(RewardCenterConstants.ConfigResourceName);
            if (_config == null)
            {
                RewardCenterLogger.LogWarning(ClassName, "Could not find RewardCenterConfig resource. Using default values.");
                _config = ScriptableObject.CreateInstance<RewardCenterConfig>();
            }
        }
        private static bool InitializationCheck()
        {
            if (IsInitialized) return true;
            RewardCenterLogger.LogError(ClassName, "RewardCenter is not initialized. Call RewardCenter.Initialize() first.");
            return false;
        }
        
        private static void OnDeepLinkParsedComplete(Campaign campaign)
        {
            if (campaign == null) return;
            var activeCampaign = _stateStore.GetCampaign();
            if (activeCampaign != null && activeCampaign.id == campaign.id) return;
            // New campaign
            _stateStore.ResetState();
            _stateStore.SetCampaign(campaign);
            _stateStore.Save();
            _assetDownloader.DownloadAssets(campaign, () => OnCampaignActivated?.Invoke(campaign));
        }
        
        private static void SetLogger()
        {
            RewardCenterLogger.Initialize(_config != null ? _config.logLevel: LogLevel.Warn);
        }
        
        private static void SetStateStore()
        {
            _stateStore = new StateStore();
            _stateStore.Load();
            if (!IsCampaignExpired()) return;
            _stateStore.ResetState();
            _stateStore.Save();
        }

        private static bool IsCampaignExpired()
        {
            if (_stateStore == null || _stateStore.State == null) return true;
            var campaign = _stateStore.State.activeCampaign;
            if (campaign == null) return false;
            if (string.IsNullOrEmpty(campaign.expiresAt)) return false;
            if (!DateTime.TryParse(campaign.expiresAt, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var expiresAt))
            {
                RewardCenterLogger.LogWarning(ClassName,$"Could not parse expiresAt: {campaign.expiresAt}");
                return false;
            }
            return DateTime.UtcNow > expiresAt;
        }
    }
}