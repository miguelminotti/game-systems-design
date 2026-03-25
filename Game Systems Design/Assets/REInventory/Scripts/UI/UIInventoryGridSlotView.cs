using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REInventory.UI
{
    public class UIInventoryGridSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<UIInventoryGridSlotView> OnPointerClicked;
        public event Action<UIInventoryGridSlotView> OnPointerEntered;
        public event Action<UIInventoryGridSlotView> OnPointerExited;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEntered?.Invoke(this);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExited?.Invoke(this);
        }
    }
}