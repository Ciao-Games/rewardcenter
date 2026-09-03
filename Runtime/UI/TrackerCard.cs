using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// The tracker card that lives in the studio's menu layout while a campaign is active.
    /// Fully passive: overlay coordinator drives Show/Hide/SetProgress/spotlight.
    /// Owns its own onboarding tooltip and a sorting canvas for spotlight rendering.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class TrackerCard : MonoBehaviour
    {
        private const int SpotlightSortingOrder = 101;

        [SerializeField] private Image currencyIcon;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private GameObject onboardingTooltip;

        private Canvas _canvas;
        private Button _button;

        public event Action OnViewClicked;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.overrideSorting = false;

            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(() => OnViewClicked?.Invoke());
        }

        public void Show(Sprite currencyIconSprite, float earnedAmount, float totalAmount)
        {
            if (currencyIcon != null && currencyIconSprite != null)
            {
                currencyIcon.sprite = currencyIconSprite;
                currencyIcon.preserveAspect = true;
            }

            if (amountText != null)
                amountText.text = $"{earnedAmount:0}";

            SetProgress(earnedAmount, totalAmount);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetProgress(float earnedAmount, float totalAmount)
        {
            if (amountText != null)
                amountText.text = $"{earnedAmount:0}";

            if (progressBar != null)
                progressBar.value = totalAmount > 0f ? earnedAmount / totalAmount : 0f;
        }

        public void EnableSpotlight()
        {
            if (_canvas == null) _canvas = GetComponent<Canvas>();
            if (_canvas != null)
            {
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = SpotlightSortingOrder;
            }
            if (onboardingTooltip != null) onboardingTooltip.SetActive(true);
        }

        public void DisableSpotlight()
        {
            if (_canvas == null) _canvas = GetComponent<Canvas>();
            if (_canvas != null) _canvas.overrideSorting = false;
            if (onboardingTooltip != null) onboardingTooltip.SetActive(false);
        }
    }
}