using System.IO;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    public static class AssetPaths
    {
        public static string Root => Path.Combine(Application.persistentDataPath, RewardCenterConstants.AssetsFolder);
        public static string PublisherLogo => Path.Combine(Root, RewardCenterConstants.PublisherLogoFilename);
        public static string CurrencyIcon => Path.Combine(Root, RewardCenterConstants.CurrencyIconFilename);
 
        public static void EnsureRootExists()
        {
            if (!Directory.Exists(Root)) Directory.CreateDirectory(Root);
        }
    }
}