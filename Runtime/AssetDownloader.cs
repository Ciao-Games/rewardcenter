using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// Downloads and caches remote campaign assets (publisher logo, currency icon) to Application.persistentDataPath.
    /// Runs both downloads in parallel and fires onComplete once both finish (success or failure).
    /// </summary>
    public class AssetDownloader
    {
        private const string ClassName = nameof(AssetDownloader);

        public void DownloadAssets(Campaign campaign, Action onComplete)
        {
            if (campaign == null)
            {
                onComplete?.Invoke();
                return;
            }

            AssetPaths.EnsureRootExists();

            var pending = 2;
            void OnAssetReady()
            {
                if (--pending == 0) onComplete?.Invoke();
            }

            DownloadFile(campaign.publisherLogoUrl, AssetPaths.PublisherLogo, OnAssetReady);
            DownloadFile(campaign.currencyIconUrl, AssetPaths.CurrencyIcon, OnAssetReady);
        }

        private static void DownloadFile(string url, string localPath, Action onDone)
        {
            if (string.IsNullOrEmpty(url))
            {
                onDone?.Invoke();
                return;
            }

            if (File.Exists(localPath))
            {
                onDone?.Invoke();
                return;
            }

            var request = UnityWebRequest.Get(url);
            request.timeout = RewardCenterConstants.HttpTimeoutSeconds;

            request.SendWebRequest().completed += _ =>
            {
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                        RewardCenterLogger.LogWarning(ClassName, $"Asset fetch failed for {url}: {request.error}");
                    else
                        File.WriteAllBytes(localPath, request.downloadHandler.data);
                }
                catch (Exception e)
                {
                    RewardCenterLogger.LogError(ClassName, $"Failed to save asset {url}: {e.Message}");
                }
                finally
                {
                    request.Dispose();
                    onDone?.Invoke();
                }
            };
        }
    }
}