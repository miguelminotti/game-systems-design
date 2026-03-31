using MMStdLib.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using REInventory.Core;

namespace REInventory.Unity.UI
{
    public class UIInventoryItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Injections")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Settings")]
        [SerializeField] private float _scaleMultiplier;

        public IRuntimeStorable BindedItem => _bindedItem;

        private Canvas _bindedCanvas;
        private IRuntimeStorable _bindedItem;
        private InventoryAidTextChangeEvent _event;
        private bool _isDragging;

        public event Action<UIInventoryItemView> OnPointerClicked;

        private void Awake()
        {
            _event = new InventoryAidTextChangeEvent("Default text");
        }

        private void Update()
        {
            UpdateDraggingState();
        }

        public void SetBindedCanvas(Canvas canvas)
        {
            _bindedCanvas = canvas;
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
            if (_bindedItem != null)
                _bindedItem.OnRotationChanged -= OnBindedItemRotationChangedHandler;
            _bindedItem = bindeddItem;
            _bindedItem.OnRotationChanged += OnBindedItemRotationChangedHandler;
            //_image.sprite = _bindedItem.BaseItem.Icon; TODO: Get from a database of icons based on id
            RefreshBindedItemPosition();
        }

        private void RefreshBindedItemPosition()
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _bindedItem.Width * _scaleMultiplier);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _bindedItem.Height * _scaleMultiplier);
            _rectTransform.anchoredPosition = new Vector2(_bindedItem.StartingPosition.X * _scaleMultiplier, -_bindedItem.StartingPosition.Y * _scaleMultiplier);
        }

        public void StartDragging()
        {
            _isDragging = true;
            _canvasGroup.alpha = .5f;
        }

        public void EndDragging()
        {
            _isDragging = false;
            _canvasGroup.alpha = 1f;
        }

        private void UpdateDraggingState()
        {
            if (!_isDragging) return;

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _bindedCanvas.transform as RectTransform,
            mouseScreenPosition,
            _bindedCanvas.worldCamera,
            out Vector2 localPoint
            );

            _rectTransform.localPosition = localPoint;
        }

        private void OnBindedItemRotationChangedHandler(IRuntimeStorable.Rotation targetRotation)
        {
            RefreshBindedItemPosition();
        }
    }
}