using System;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// The welcome popup shown on first activation of a campaign.
    /// Sits on the WelcomePopup child of the RewardCenterUI prefab.
    /// </summary>
    public class WelcomePopup : MonoBehaviour
    {
        [SerializeField] private Image publisherLogo;
        [SerializeField] private Image appLogo;    // studio assigns their app logo sprite
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject plusIcon;

        public event Action OnContinueClicked;

        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());

        }
        
        public void Show(Sprite publisherLogoSprite)
        {
            if (publisherLogo != null)
            {
                publisherLogo.sprite = publisherLogoSprite;
                publisherLogo.preserveAspect = true;
                publisherLogo.gameObject.SetActive(publisherLogoSprite != null);
            }

            var hasAppLogo = appLogo != null && appLogo.sprite != null;
            if (appLogo != null) appLogo.gameObject.SetActive(hasAppLogo);
            if (plusIcon != null) plusIcon.SetActive(publisherLogoSprite != null && hasAppLogo);

            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

