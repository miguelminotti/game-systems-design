using MMStdLib.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REInventory.UI
{
    public class UIInventoryItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Injections")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;

        [Header("Settings")]
        [SerializeField] private float _scaleMultiplier;

        public IRuntimeStorable BindedItem => _bindedItem;

        private IRuntimeStorable _bindedItem;
        private InventoryAidTextChangeEvent _event;

        public event Action<UIInventoryItemView> OnPointerClicked;

        private void Awake()
        {
            _event = new InventoryAidTextChangeEvent("Default text");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _event.NewText = $"{_bindedItem.BaseItem.Name}\n\n{_bindedItem.BaseItem.Description}";
            GameEventBus.Publish((IInventoryAidTextChangeEvent)_event);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _event.NewText = "";
            GameEventBus.Publish((IInventoryAidTextChangeEvent)_event);
        }

        public void SetBindedItem(IRuntimeStorable bindeddItem)
        {
            _bindedItem = bindeddItem;
            _image.sprite = _bindedItem.BaseItem.Icon;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _bindedItem.BaseItem.Width * _scaleMultiplier);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _bindedItem.BaseItem.Height * _scaleMultiplier);
            _rectTransform.anchoredPosition = new Vector2(_bindedItem.XPosition * _scaleMultiplier, -_bindedItem.YPosition * _scaleMultiplier);
        }
    }
}