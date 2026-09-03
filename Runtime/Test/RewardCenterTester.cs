using System;
using System.IO;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// Runtime test helpers. Callable from Editor tools and from in-game QA buttons alike.
    /// Wire an in-game button to <see cref="SendInlineDeepLink"/> passing a
    /// <see cref="RewardCenterTestData"/> asset for on-device testing.
    /// </summary>
    public class RewardCenterTester : MonoBehaviour
    {
        [SerializeField] private RewardCenterTestData _testData;

        public void InitializeSDK()
        {
            RewardCenter.Initialize();
        }
        
        public void SendInlineDeepLink()
        {
            if (_testData == null || string.IsNullOrEmpty(_testData.inlineUrl)) return;
            RewardCenter.HandleDeepLink(_testData.inlineUrl);
        }

        public void SendReferenceDeepLink()
        {
            if (_testData == null || string.IsNullOrEmpty(_testData.referenceUrl)) return;
            RewardCenter.HandleDeepLink(_testData.referenceUrl);
        }

        public void ClearState()
        {
            var path = Path.Combine(Application.persistentDataPath, RewardCenterConstants.StateFileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[Tester] Cleared state at {path}");
            }
            else
            {
                Debug.Log("[Tester] No state file to clear.");
            }
        }

        public void SendEvent(int index)
        {
            var milestones = RewardCenter.State?.activeCampaign?.milestones;
            if (milestones == null || milestones.Count == 0)
            {
                Debug.LogWarning("[Tester] No active campaign or no milestones.");
                return;
            }
            if (index < 0 || index >= milestones.Count)
            {
                Debug.LogWarning($"[Tester] Milestone index {index} out of range (0-{milestones.Count - 1}).");
                return;
            }

            var milestone = milestones[index];
            RewardCenter.ReportEvent(milestone.eventName, milestone.targetValue);
        }
    }
}