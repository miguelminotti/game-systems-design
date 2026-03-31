using MMStdLib.Utils;
using UnityEngine;
using REInventory.Core;

namespace REInventory.Unity.UI
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

        public void Initialize()
        {
            _itemViews = _itemViewsParent.GetComponentsInChildren<UIInventoryItemView>();
            _gridSlots = _gridSlotsParent.GetComponentsInChildren<UIInventoryGridSlotView>();

            foreach (var gridSlot in _gridSlots)
            {
                gridSlot.OnPointerClicked += OnGridSlotClicked;
                gridSlot.OnPointerExited += OnGridSlotPointerEnteredHandler;
            }

            foreach (var itemView in _itemViews)
            {
                itemView.OnPointerClicked += OnItemViewPointerClickedHandler;
                itemView.SetBindedCanvas(_parentCanvas);
            }

            _tempDraggingItemView.SetBindedCanvas(_parentCanvas);
            _tempDraggingItemView.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEventBus.Subscribe<InventoryEvents.IInventoryChangedEvent>(OnInventoryChangedHandler);
            GameEventBus.Subscribe<InventoryEvents.IInventoryClearedEvent, IInventoryCore>(OnInventoryClearedHandler);
            GameEventBus.Subscribe<IRuntimeStorable.IMoveInventoryItemEvent>(OnMoveInventoryItemHandler);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<InventoryEvents.IInventoryChangedEvent>(OnInventoryChangedHandler);
            GameEventBus.Unsubscribe<InventoryEvents.IInventoryClearedEvent, IInventoryCore>(OnInventoryClearedHandler);
            GameEventBus.Unsubscribe<IRuntimeStorable.IMoveInventoryItemEvent>(OnMoveInventoryItemHandler);
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

        private void SetState(GridInteractionState newState)
        {
            _currentInteractionState = newState;
        }

        private void OnGridSlotClicked(UIInventoryGridSlotView gridSlotView)
        {
            if (_currentInteractionState != GridInteractionState.ItemDragging) return;

            if (TryPlaceItemOnGridSlot(gridSlotView))
            {
                SetState(GridInteractionState.Idle);
                _tempDraggingItemView.EndDragging();
                _tempDraggingItemView.gameObject.SetActive(false);

            }
        }

        private bool TryPlaceItemOnGridSlot(UIInventoryGridSlotView gridSlotView)
        {
            if (gridSlotView == null || _bindedInventory == null) return false;

            if (_bindedInventory.AddItemAtPosition(_tempDraggingItemView.BindedItem, new GridPosition(gridSlotView.XPosition, gridSlotView.YPosition)) == IInventoryGrid.PlaceItemResult.Succeeded) {
                Debug.Log($"Item placed at: {gridSlotView.XPosition},{gridSlotView.YPosition}");
                return true;
            }

            Debug.Log("Cant place item");
            return false;
        }

        private void OnGridSlotPointerEnteredHandler(UIInventoryGridSlotView gridSlotView)
        {
            if (_currentInteractionState != GridInteractionState.ItemDragging) return;

            // Move the item to the position
        }

        private void OnItemViewPointerClickedHandler(UIInventoryItemView itemView)
        {
            if (_currentInteractionState != GridInteractionState.Idle) return;
            _inventoryItemOptionsView.RefreshOptions(itemView.BindedItem.Options);
            _inventoryItemOptionsView.OpenOptions();
        }

        private void OnInventoryChangedHandler(InventoryEvents.IInventoryChangedEvent eventData)
        {
            if (eventData.RefInventory != _bindedInventory) return;
            DrawItems();
        }

        private void OnInventoryClearedHandler(IInventoryCore refInventory)
        {
            if (refInventory != _bindedInventory) return;
            DrawItems();
        }

        private void OnMoveInventoryItemHandler(IRuntimeStorable.IMoveInventoryItemEvent eventData)
        {
            if (eventData.RefInventory != _bindedInventory || eventData.Item == null) return;

            SetState(GridInteractionState.ItemDragging);
            _tempDraggingItemView.gameObject.SetActive(true);
            _tempDraggingItemView.SetBindedItem(eventData.Item);
            _tempDraggingItemView.StartDragging();
        }
    }
}