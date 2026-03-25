using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REInventory.UI
{
    public class UIInventoryItemOption : MonoBehaviour, IPointerClickHandler
    {
        [Header("Injections")]
        [SerializeField] private TextMeshProUGUI _optionText;

        private IStorableOption _bindedStorableOption;

        public void OnPointerClick(PointerEventData eventData)
        {
            _bindedStorableOption.Submit();
        }

        public void Refresh(IStorableOption storableOption)
        {
            _bindedStorableOption = storableOption;
            _optionText.text = storableOption.OptionLabel;
        }
    }
}