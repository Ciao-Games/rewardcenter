using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// Interactive chest in the CelebrationPopup. Slides in from bottom, idles with
    /// a bounce, player taps 3 times (each tap = punch scale + fill an indicator),
    /// final tap swaps to opened chest, holds for celebration, then jumps up and
    /// slides offscreen. Fires OnChestOpened (for confetti timing) and
    /// OnChestExited (for reward reveal timing).
    /// </summary>
    public class ChestView : MonoBehaviour
    {
        private const int RequiredTaps = 3;

        [Header("Entrance")]
        [SerializeField] private float entranceOffscreenY = -800f;
        [SerializeField] private float entranceDuration = 0.6f;

        [Header("Idle")]
        [SerializeField] private float bounceAmplitude = 10f;
        [SerializeField] private float bouncePeriod = 0.8f;

        [Header("Tap Feedback")]
        [SerializeField] private float tapPunchAmount = 0.3f;
        [SerializeField] private float tapPunchDuration = 0.2f;
        [SerializeField] private float shakeAmount = 12f;
        [SerializeField] private float shakeDuration = 0.4f;

        [Header("Chest Opening")]
        [SerializeField] private float openScaleUpDuration = 0.4f;
        [SerializeField] private float celebrationHoldDuration = 0.5f;

        [Header("Chest Exit")]
        [SerializeField] private float jumpUpY = 100f;
        [SerializeField] private float jumpDuration = 0.3f;
        [SerializeField] private float exitOffscreenY = -2000f;
        [SerializeField] private float exitDuration = 0.5f;

        [Header("Bindings")]
        [SerializeField] private Button chestButton;
        [SerializeField] private GameObject closedChest;
        [SerializeField] private GameObject openedChest;
        [SerializeField] private CiaoTweener chestTweener;
        [SerializeField] private GameObject tapParent;
        [SerializeField] private Transform indicatorsParent;
        [SerializeField] private Sprite indicatorEmpty;
        [SerializeField] private Sprite indicatorFilled;
        [SerializeField] private GameObject celebrationVisuals;
        private int _tapCount;

        public event Action OnChestOpened;
        public event Action OnChestExited;

        private void Awake()
        {
            if (chestButton != null)
                chestButton.onClick.AddListener(OnChestTapped);
        }

        public void Show()
        {
            _tapCount = 0;
            ResetVisuals();
            if (tapParent != null) tapParent.SetActive(true);
            gameObject.SetActive(true);

            if (chestButton != null) chestButton.interactable = false;
            StartCoroutine(EntranceRoutine());
        }

        public void Hide()
        {
            if (chestTweener != null) chestTweener.StopBounce();
            gameObject.SetActive(false);
        }

        private void ResetVisuals()
        {
            if (closedChest != null) closedChest.SetActive(true);
            if (openedChest != null) openedChest.SetActive(false);
            if (celebrationVisuals != null) celebrationVisuals.SetActive(false);
            SetAllIndicators(indicatorEmpty);
        }

        private IEnumerator EntranceRoutine()
        {
            if (chestTweener == null) yield break;

            chestTweener.transform.localPosition = new Vector3(0f, entranceOffscreenY, 0f);
            yield return chestTweener.MoveTo(Vector3.zero, entranceDuration);

            chestTweener.StartBounce(bounceAmplitude, bouncePeriod);
            if (chestButton != null) chestButton.interactable = true;
        }

        private void OnChestTapped()
        {
            _tapCount++;

            if (indicatorsParent != null && _tapCount <= indicatorsParent.childCount)
            {
                var img = indicatorsParent.GetChild(_tapCount - 1).GetComponent<Image>();
                if (img != null && indicatorFilled != null) img.sprite = indicatorFilled;
            }

            if (chestTweener != null)
                chestTweener.PunchScale(Vector3.one * tapPunchAmount, tapPunchDuration);
            if (celebrationVisuals != null)
            {
                celebrationVisuals.SetActive(false); // this resets the anim
                celebrationVisuals.SetActive(true);
            }
            if (_tapCount >= RequiredTaps)
                StartCoroutine(FinalTapSequence());
        }

        private IEnumerator FinalTapSequence()
        {
            if (chestButton != null) chestButton.interactable = false;
            if (tapParent != null) tapParent.SetActive(false);
            // Let the tap punch finish
            yield return new WaitForSeconds(tapPunchDuration);

            // Stop bounce so shake has clean position control
            if (chestTweener != null) chestTweener.StopBounce();

            // Tremor before opening
            if (chestTweener != null)
                yield return chestTweener.Shake(shakeAmount, shakeDuration);

            // Swap to opened
            if (closedChest != null) closedChest.SetActive(false);
            if (openedChest != null) openedChest.SetActive(true);
            OnChestOpened?.Invoke();
            if (chestTweener != null)
            {
                chestTweener.transform.localScale = Vector3.one * 0.8f;
                yield return chestTweener.Scale(Vector3.one, openScaleUpDuration);
            }

            

            yield return new WaitForSeconds(celebrationHoldDuration);

            if (chestTweener != null)
            {
                yield return chestTweener.MoveTo(new Vector3(0f, jumpUpY, 0f), jumpDuration);
                yield return chestTweener.MoveTo(new Vector3(0f, exitOffscreenY, 0f), exitDuration);
            }

            gameObject.SetActive(false);
            OnChestExited?.Invoke();
        }

        private void SetAllIndicators(Sprite sprite)
        {
            if (indicatorsParent == null || sprite == null) return;
            foreach (Transform child in indicatorsParent)
            {
                var img = child.GetComponent<Image>();
                if (img != null) img.sprite = sprite;
            }
        }
    }
}