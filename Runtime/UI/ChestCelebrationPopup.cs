using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// Full-screen celebration shown when a milestone is completed.
    /// Chest enters, player taps 3 times, chest opens (fires confetti), chest exits
    /// (fires reward reveal + continue button).
    /// </summary>
    public class ChestCelebrationPopup : CelebrationPopupBase
    {
        private const float ContinueButtonDelay = 0.5f;

        [SerializeField] private ChestView chestView;
        [SerializeField] private GameObject celebrationVisuals;
        [SerializeField] private Image rewardCurrencyIcon;
        [SerializeField] private TMP_Text rewardAmountText;
        [SerializeField] private GameObject rewardReveal;
        [SerializeField] private Button continueButton;
        
        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(RaiseContinueClicked);

            if (chestView != null)
            {
                chestView.OnChestOpened += OnChestOpened;
                chestView.OnChestExited += OnChestExited;
            }
        }

        private void OnDestroy()
        {
            if (chestView != null)
            {
                chestView.OnChestOpened -= OnChestOpened;
                chestView.OnChestExited -= OnChestExited;
            }
        }

        public override void Show(Milestone milestone, string currencyName, Sprite currencyIconSprite)
        {
            if (milestone == null) return;

            if (rewardCurrencyIcon != null && currencyIconSprite != null)
                rewardCurrencyIcon.sprite = currencyIconSprite;

            if (rewardAmountText != null)
                rewardAmountText.text = $"+{milestone.rewardAmount:0}";

            if (rewardReveal != null) rewardReveal.SetActive(false);
            if (celebrationVisuals != null) celebrationVisuals.SetActive(false);
            if (continueButton != null) continueButton.gameObject.SetActive(false);

            gameObject.SetActive(true);
            if (chestView != null) chestView.Show();
        }

        public override void Hide()
        {
            if (chestView != null) chestView.Hide();
            if (celebrationVisuals != null) celebrationVisuals.SetActive(false);
            if (rewardReveal != null) rewardReveal.SetActive(false);
            if (continueButton != null) continueButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnChestOpened()
        {
            if (celebrationVisuals == null) return;
            celebrationVisuals.SetActive(true);
        }

        private void OnChestExited()
        {
            StartCoroutine(RewardRevealSequence());
        }

        private IEnumerator RewardRevealSequence()
        {
            if (rewardReveal != null) rewardReveal.SetActive(true);
            yield return new WaitForSeconds(ContinueButtonDelay);
            if (continueButton != null) continueButton.gameObject.SetActive(true);
        }
    }
}