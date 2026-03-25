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
        [SerializeField] private UIInventoryItemOptionsView _inventoryItemOptionsView;
        [SerializeField] private Transform _itemViewsParent;
        [SerializeField] private Transform _gridSlotsParent;

        private GridInteractionState _currentInteractionState = GridInteractionState.Idle;
        private IInventoryCore _subjectInventory;
        private UIInventoryItemView[] _itemViews;
        private UIInventoryGridSlotView[] _gridSlots;

        private void Awake()
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
            }
        }

        public void BindInventory(IInventoryCore inventory)
        {
            _subjectInventory = inventory;
        }

        public void DrawItems()
        {
            for (int i = 0; i < _itemViews.Length; i++)
            {
                if (i < _subjectInventory.Items.Count)
                {
                    _itemViews[i].SetBindedItem(_subjectInventory.Items[i]);
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

            // Confirm moving the item to the position
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
    }
}