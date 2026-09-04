using System.IO;
using UnityEditor;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// Editor tester for the Reward Center SDK. Menu: Ciao Games > Reward Center > Test Window.
    /// Delegates all work to <see cref="RewardCenterTester"/> so behavior is identical to
    /// in-game QA buttons.
    /// </summary>
    public class RewardCenterTestWindow : EditorWindow
    {
        private const string StateFileName = "reward_center_state.json";
        private const string AssetsFolder = "rewardcenter/assets";

        [SerializeField] private RewardCenterTestData _testData;
        private string _eventName = "level_completed";

        [MenuItem("Ciao Games/Reward Center/Test Window")]
        public static void ShowWindow()
        {
            GetWindow<RewardCenterTestWindow>("Reward Center Tester");
        }

        [MenuItem("Ciao Games/Reward Center/Clear State")]
        public static void ClearStateMenu()
        {
            ClearState();
        }

        [MenuItem("Ciao Games/Reward Center/Open State File")]
        public static void OpenStateFile()
        {
            var path = Path.Combine(Application.persistentDataPath, StateFileName);
            EditorUtility.RevealInFinder(File.Exists(path) ? path : Application.persistentDataPath);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Reward Center Tester", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _testData = (RewardCenterTestData)EditorGUILayout.ObjectField(
                "Test Data", _testData, typeof(RewardCenterTestData), false);

            if (_testData == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a RewardCenterTestData asset. Create one via Assets > Create > Ciao Games > Reward Center Test Data.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play mode to send SDK calls.", MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Initialize SDK"))
                {
                    RewardCenter.Initialize();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Deep Links", EditorStyles.miniBoldLabel);

                using (new EditorGUI.DisabledScope(_testData == null))
                {
                    if (GUILayout.Button("Send inline test link"))
                        RewardCenter.HandleDeepLink(_testData.inlineUrl);
                    
                    if (GUILayout.Button("Send reference test link"))
                        RewardCenter.HandleDeepLink(_testData.referenceUrl);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Report Event", EditorStyles.miniBoldLabel);
                _eventName = EditorGUILayout.TextField("Event Name", _eventName);
                if (GUILayout.Button("Report Event"))
                {
                    RewardCenter.ReportEvent(_eventName);
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Clear State"))
            {
                ClearState();
            }
        }
        
        public static void ClearState()
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
    }
}