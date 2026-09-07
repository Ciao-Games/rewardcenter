using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ciao.RC.UI
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

            bool hasIcon = currencyIcon != null && currencyIconSprite != null;
            if (hasIcon)
            {
                currencyIcon.sprite = currencyIconSprite;
                currencyIcon.gameObject.SetActive(true);
            } 
            else if (currencyIcon != null)
            {
                currencyIcon.gameObject.SetActive(false);
            }
            
            if (rewardText != null)
            {
                rewardText.text = hasIcon
                    ? $"+{milestone.rewardAmount:N0}"
                    : $"+{milestone.rewardAmount:0} {RewardCenter.CurrencyName}";
            }
            
            
           
            if (notCompletedIndicator != null)
                notCompletedIndicator.SetActive(!milestone.isCompleted);
            if (completedIndicator != null)
                completedIndicator.SetActive(milestone.isCompleted);
        }
    }
}


