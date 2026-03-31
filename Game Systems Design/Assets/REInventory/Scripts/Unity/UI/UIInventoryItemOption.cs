using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using REInventory.Core;

namespace REInventory.Unity.UI
{
    public class UIInventoryItemOption : MonoBehaviour, IPointerClickHandler
    {
        [Header("Injections")]
        [SerializeField] private TextMeshProUGUI _optionText;

        private IStorableOption _bindedStorableOption;

        public event Action<UIInventoryItemOption> OnOptionClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            _bindedStorableOption.Submit();
            OnOptionClicked?.Invoke(this);
        }

        public void Refresh(IStorableOption storableOption)
        {
            _bindedStorableOption = storableOption;
            _optionText.text = storableOption.OptionLabel;
        }
    }
}