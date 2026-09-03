using System;
using System.IO;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    /// <summary> Loads and saves <see cref="RewardCenterState"/> as JSON at Application.persistentDataPath. </summary>
    public class StateStore
    {
        private const string ClassName = nameof(StateStore);
        private readonly string _path;
        public RewardCenterState State { get; private set; }
        public void ResetState() => State = new RewardCenterState();
        public Campaign GetCampaign() => State.activeCampaign;
        public void SetCampaign(Campaign campaign) => State.activeCampaign = campaign;
        public void SetWelcomeShown(bool value) => State.hasShownWelcome = value;
        public void SetOnboardingShown(bool value) => State.hasShownOnboarding = value;

        public StateStore()
        {
            _path = Path.Combine(Application.persistentDataPath, RewardCenterConstants.StateFileName);
        }
        

        /// <summary> Reads state from disk. If no file exists or the file is corrupted, initializes fresh state. </summary>
        public void Load()
        {
            if (!File.Exists(_path))
            {
                ResetState();
                return;
            }

            try
            {
                var json = File.ReadAllText(_path);
                State = JsonUtility.FromJson<RewardCenterState>(json) ?? new RewardCenterState();
            }
            catch (Exception e)
            {
                RewardCenterLogger.LogWarning(ClassName,
                    $"Failed to load state from {_path} . Creating new state. Error: {e.Message}");
                ResetState();
            }
        }

        /// <summary> Saves state to disk.</summary>
        public void Save()
        {
            if (State == null)
                return;

            try
            {
                var json = JsonUtility.ToJson(State, true);
                File.WriteAllText(_path, json);
            }
            catch (Exception e)
            {
                RewardCenterLogger.LogError(ClassName, $"Failed to save state to {_path}. Error: {e.Message}");
            }
        }

        /// <summary> Deletes the state file and resets State</summary>
        public void Clear()
        {
            ResetState();
            try
            {
                if (File.Exists(_path))
                    File.Delete(_path);
            }
            catch (Exception e)
            {
                RewardCenterLogger.LogWarning(ClassName, $"Failed to delete state file {_path}. Error: {e.Message}");
            }
        }
    }
}