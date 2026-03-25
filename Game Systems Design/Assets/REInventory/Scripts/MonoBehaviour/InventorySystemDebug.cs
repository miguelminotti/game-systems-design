using UnityEngine;

namespace REInventory
{
    [RequireComponent(typeof(InventorySystem))]
    public class InventorySystemDebug : MonoBehaviour
    {
        [Header("Place item settings")]
        [SerializeField] private StorableItem _item;
        [SerializeField] private int _itemWidth = 3;
        [SerializeField] private int _itemHeight = 3;
        [SerializeField] private int _xPosition = 0;
        [SerializeField] private int _yPosition = 0;

        private InventorySystem _inventory;

        private void Awake()
        {
            _inventory = GetComponent<InventorySystem>();
        }

        [ContextMenu("Place item")]
        public void PlaceItem()
        {
            IStorable newItem = _item;
            IRuntimeStorable runtimeStorableItem = newItem.GetRuntimeInstance();
            if (_inventory.InventoryCore.AddItemAtPosition(runtimeStorableItem, _xPosition, _yPosition))
            {
                Debug.Log($"Item of size {_itemWidth}x{_itemHeight} placed in inventory.");
            } else
            {
                Debug.Log($"Failed to place item of size {_itemWidth}x{_itemHeight} in inventory. Not enough space.");
            }
        }
    }
}