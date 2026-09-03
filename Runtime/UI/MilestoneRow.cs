using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// One row in the Milestones Map list. when the map is opened.
    /// </summary>
    public class MilestoneRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Image currencyIcon;
        [SerializeField] private GameObject completedIndicator;
        [SerializeField] private GameObject notCompletedIndicator;
        [SerializeField] private Color completedTextColor;
        [SerializeField] private Color notCompletedTextColor;



        public void Populate(Milestone milestone, int displayIndex, Sprite currencyIconSprite)
        {
            if (milestone == null) return;

            if (descriptionText != null)
            {
                descriptionText.text = !string.IsNullOrEmpty(milestone.description)
                    ? milestone.description
                    : $"Complete Milestone #{displayIndex}";
                descriptionText.color = milestone.isCompleted ? completedTextColor : notCompletedTextColor;
            }

            if (rewardText != null)
                rewardText.text = $"+{milestone.rewardAmount:N0} ";
            
            if (currencyIcon != null)
            {
                if (currencyIconSprite != null)
                    currencyIcon.sprite = currencyIconSprite;
                currencyIcon.preserveAspect = true;
            }
            if (notCompletedIndicator != null)
                notCompletedIndicator.SetActive(!milestone.isCompleted);
            if (completedIndicator != null)
                completedIndicator.SetActive(milestone.isCompleted);
        }
    }
}


