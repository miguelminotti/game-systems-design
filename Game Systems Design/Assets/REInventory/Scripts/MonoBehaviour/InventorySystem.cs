using UnityEngine;

namespace REInventory
{
    public class InventorySystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int gridWidth = 5;
        [SerializeField] private int gridHeight = 5;

        public IInventoryCore InventoryCore => _inventoryCore;

        private IInventoryCore _inventoryCore;

        private void Awake()
        {
            _inventoryCore = new InventoryCore();
            _inventoryCore.Initialize(gridWidth, gridHeight);
        }
    }
}