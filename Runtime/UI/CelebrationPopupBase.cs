using System;
using UnityEngine;

namespace Ciao.RC.UI
{
    public abstract class CelebrationPopupBase : MonoBehaviour
    {
        public event Action OnContinueClicked;
        protected void RaiseContinueClicked() => OnContinueClicked?.Invoke();

        public abstract void Show(Milestone milestone, string currencyName, Sprite currencyIcon);
        public abstract void Hide();
    }
}