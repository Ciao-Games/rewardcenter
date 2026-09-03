using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CiaoGames.RewardCenter
{
    /// <summary>  Parses a deeplink URL: Inline & Reference mode
    /// - Inline: campaign JSON is URL-encoded in the "payload" query param
    /// - Reference: campaign JSON is hosted at the URL in "config_url"
    /// </summary>
    public class DeepLinkHandler
    {
        private const string ClassName = nameof(DeepLinkHandler);
        public void ParseUrl(string url, Action<Campaign> onComplete)
        {
            if (string.IsNullOrEmpty(url))
            {
                onComplete?.Invoke(null);
                return;
            }

            Dictionary<string, string> query;
            try
            {
                var uri = new Uri(url);
                query = RewardCenterUtils.ParseQuery(uri.Query);
            }
            catch (Exception e)
            {
                RewardCenterLogger.LogWarning(ClassName, $"Failed to parse URL: {url}. Error: {e.Message}");
                onComplete?.Invoke(null);
                return;
            }

            // Check if valid deeplink
            if (!query.ContainsKey(RewardCenterConstants.QueryCampaignId))
            {
                onComplete?.Invoke(null);
                return;
            }

            // Check if it is Reference mode
            if (query.TryGetValue(RewardCenterConstants.QueryConfigUrl, out var configUrl))
            {
                ParseReference(configUrl, onComplete);
                return;
            }
            
            // Check if it is Inline mode
            if (query.TryGetValue(RewardCenterConstants.QueryPayload, out var inlinePayload))
            {
                ParseInline(inlinePayload, onComplete);
                return;
            }
            
            RewardCenterLogger.LogWarning(ClassName, $"Invalid URL: {url}");
            onComplete?.Invoke(null);
        }
        
        private static void ParseInline(string inlinePayload, Action<Campaign> onComplete)
        {
            try
            {
                var json = Uri.UnescapeDataString(inlinePayload);
                var payload = JsonUtility.FromJson<CampaignPayload>(json);
                onComplete?.Invoke(PayloadMapper.ToCampaign(payload));
            } 
            catch (Exception e)
            {
                RewardCenterLogger.LogError(ClassName, $"Failed to parse inline payload: {e.Message}");
                onComplete?.Invoke(null);
            }
        }

        private static void ParseReference(string configUrl, Action<Campaign> onComplete)
        {
            var request = UnityWebRequest.Get(configUrl);
            request.timeout = RewardCenterConstants.HttpTimeoutSeconds;
            var operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        RewardCenterLogger.LogWarning(ClassName, $"Failed to fetch campaign config: {request.error}");
                        onComplete?.Invoke(null);
                        return;
                    }

                    var payload = JsonUtility.FromJson<CampaignPayload>(request.downloadHandler.text);
                    onComplete?.Invoke(PayloadMapper.ToCampaign(payload));
                }
                catch (Exception e)
                {
                    RewardCenterLogger.LogError(ClassName, $"Failed to parse fetched campaign config: {e.Message}");
                    onComplete?.Invoke(null);
                }
                finally
                {
                    request.Dispose();
                }
            };
        }
    }
}