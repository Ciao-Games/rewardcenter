using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ciao.RC.UI
{
    public class SimpleCelebrationPopup : CelebrationPopupBase
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text rewardAmountText;
        [SerializeField] private Image rewardCurrencyIcon;
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject celebrationVisuals;
        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(RaiseContinueClicked);
        }
        
        public override void Show(Milestone milestone, string currencyName, Sprite currencyIconSprite)
        {
            if (milestone == null) return;

            SetDescriptionText(milestone, currencyName);
            bool hasIcon = rewardCurrencyIcon != null && currencyIconSprite != null;
            if (hasIcon)
            {
                rewardCurrencyIcon.sprite = currencyIconSprite;
                rewardCurrencyIcon.gameObject.SetActive(true);
            }else if (rewardCurrencyIcon != null)
            {
                rewardCurrencyIcon.gameObject.SetActive(false);
            }
           
            if (rewardAmountText != null)
            {
                rewardAmountText.text = hasIcon
                    ? $"+{milestone.rewardAmount:0}"
                    : $"+{milestone.rewardAmount:0} {currencyName}";
            }

            if (celebrationVisuals != null)
            {
                celebrationVisuals.SetActive(false);
                celebrationVisuals.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            if (celebrationVisuals != null) celebrationVisuals.SetActive(false);
            gameObject.SetActive(false);
        }

        private void SetDescriptionText(Milestone milestone, string currencyName)
        {
            if (descriptionText != null)
                descriptionText.text = $"Completed '{milestone.description}' and earned {milestone.rewardAmount:0} {currencyName}!";
        }
    }
}