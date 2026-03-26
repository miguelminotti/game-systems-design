using MMStdLib.Utils;
using System;
using UnityEngine;

namespace REInventory.UI
{
    public class UIInventoryGridController : MonoBehaviour
    {
        public enum GridInteractionState
        {
            Idle,
            ItemDragging,
        }

        [Header("Injections")]
        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private UIInventoryItemOptionsView _inventoryItemOptionsView;
        [SerializeField] private Transform _itemViewsParent;
        [SerializeField] private Transform _gridSlotsParent;
        [SerializeField] private UIInventoryItemView _tempDraggingItemView;

        private GridInteractionState _currentInteractionState = GridInteractionState.Idle;
        private IInventoryCore _bindedInventory;
        private UIInventoryItemView[] _itemViews;
        private UIInventoryGridSlotView[] _gridSlots;
        private UIInventoryItemView _currentDraggingItemView;

        public void Initialize()
        {
            _itemViews = _itemViewsParent.GetComponentsInChildren<UIInventoryItemView>();
            _gridSlots = _gridSlotsParent.GetComponentsInChildren<UIInventoryGridSlotView>();

            foreach (var gridSlot in _gridSlots)
            {
                gridSlot.OnPointerClicked += OnGridSlotClicked;
                gridSlot.OnPointerExited += OnGridSlotPointerEntered;
            }

            foreach (var itemView in _itemViews)
            {
                itemView.OnPointerClicked += OnItemViewPointerClicked;
                itemView.SetBindedCanvas(_parentCanvas);
            }

            _tempDraggingItemView.SetBindedCanvas(_parentCanvas);
        }

        private void OnEnable()
        {
            GameEventBus.Subscribe<IInventoryChangedEvent>(OnInventoryChangedHandler);
            GameEventBus.Subscribe<IMoveInventoryItemEvent>(OnMoveInventoryItemHandler);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<IInventoryChangedEvent>(OnInventoryChangedHandler);
            GameEventBus.Unsubscribe<IMoveInventoryItemEvent>(OnMoveInventoryItemHandler);
        }

        public void BindInventory(IInventoryCore inventory)
        {
            _bindedInventory = inventory;
            // Setup grid slots with inventory dimensions
            for (int i = 0; i < _gridSlots.Length; i++)
            {
                UIInventoryGridSlotView gridSlot = _gridSlots[i];
                gridSlot.Setup(i % inventory.Width, i / inventory.Width);
            }
        }

        public void DrawItems()
        {
            for (int i = 0; i < _itemViews.Length; i++)
            {
                if (i < _bindedInventory.Items.Count)
                {
                    _itemViews[i].SetBindedItem(_bindedInventory.Items[i]);
                    _itemViews[i].gameObject.SetActive(true);
                }
                else
                {
                    _itemViews[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetState(GridInteractionState newState)
        {
            _currentInteractionState = newState;
        }

        private void OnGridSlotClicked(UIInventoryGridSlotView gridSlotView)
        {
            if (_currentInteractionState != GridInteractionState.ItemDragging) return;

            if (TryPlaceItemOnGridSlot(gridSlotView))
            {
                SetState(GridInteractionState.Idle);
                _currentDraggingItemView.EndDragging();
                _currentDraggingItemView = null;
            }
        }

        private bool TryPlaceItemOnGridSlot(UIInventoryGridSlotView gridSlotView)
        {
            if (gridSlotView == null || _bindedInventory == null) return false;

            if (_bindedInventory.AddItemAtPosition(_currentDraggingItemView.BindedItem, gridSlotView.XPosition, gridSlotView.YPosition)) {
                Debug.Log($"Item placed at: {gridSlotView.XPosition},{gridSlotView.YPosition}");
                return true;
            }

            Debug.Log("Cant place item");
            return false;
        }

        private void OnGridSlotPointerEntered(UIInventoryGridSlotView gridSlotView)
        {
            if (_currentInteractionState != GridInteractionState.ItemDragging) return;

            // Move the item to the position
        }

        private void OnItemViewPointerClicked(UIInventoryItemView itemView)
        {
            _inventoryItemOptionsView.RefreshOptions(itemView.BindedItem.Options);
            _inventoryItemOptionsView.OpenOptions();
        }

        private void OnInventoryChangedHandler(IInventoryChangedEvent eventData)
        {
            if (eventData.RefInventory != _bindedInventory) return;
            DrawItems();
        }

        private void OnMoveInventoryItemHandler(IMoveInventoryItemEvent eventData)
        {
            if (eventData.RefInventory != _bindedInventory || eventData.Item == null) return;

            SetState(GridInteractionState.ItemDragging);
            _tempDraggingItemView.SetBindedItem(eventData.Item);
            _tempDraggingItemView.StartDragging();
        }
    }
}