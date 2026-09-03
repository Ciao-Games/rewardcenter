using UnityEngine;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// Reusable test data for SDK integration testing. Create instances via
    /// Assets > Create > Ciao Games > Reward Center Test Data.
    /// Point the Test Window at one, or pass it to RewardCenterTester methods
    /// from your own dev/QA buttons.
    /// </summary>
    [CreateAssetMenu(fileName = "RewardCenterTestData", menuName = "Ciao Games/Reward Center Test Data")]
    public class RewardCenterTestData : ScriptableObject
    {
        [Header("Deep Links")]
        public string inlineUrl;
        public string referenceUrl;
    }
}