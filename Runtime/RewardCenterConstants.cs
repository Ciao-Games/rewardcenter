namespace Ciao.RC
{
    public class RewardCenterConstants
    {
        // Local Storage
        public const string StateFileName = "reward_center_state.json";
        public const string PublisherLogoFilename = "publisher_logo";
        public const string CurrencyIconFilename = "currency_icon";
        public const string AssetsFolder = "rewardcenter/assets";
        public const string ConfigResourceName = "RewardCenterConfig";
        public const string TestDataFileName = "RewardCenterTestData";
        
        // HTTP
        public const string QueryCampaignId = "cid";
        public const string QueryPayload = "payload";
        public const string QueryConfigUrl = "config_url";
        public const int HttpTimeoutSeconds = 5;
        
        // Assets
        public const string PrefabsSourceDir = "Packages/com.ciaogames.rewardcenter/Runtime/Prefabs";
        public const string PrefabsDestDir = "Assets/RewardCenter/Prefabs";
        public const string ResourcesDir = "Assets/RewardCenter/Resources";
        
        // Documentation
        public const string DocumentationUrl = "https://docs.ciao.games/docs/reward-center";
        
    }
}