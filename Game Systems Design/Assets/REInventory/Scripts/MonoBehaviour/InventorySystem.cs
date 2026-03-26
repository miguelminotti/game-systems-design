using MMStdLib.Utils;
using REInventory.UI;
using UnityEngine;

namespace REInventory
{
    public class InventorySystem : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private UIInventoryCore uIInventoryCore;

        [Header("Settings")]
        [SerializeField] private int gridWidth = 5;
        [SerializeField] private int gridHeight = 5;

        public IInventoryCore InventoryCore => _inventoryCore;

        private IInventoryCore _inventoryCore;

        private void Awake()
        {
            _inventoryCore = new InventoryCore();
            _inventoryCore.Initialize(gridWidth, gridHeight);
            ServiceLocator.RegisterService(InventoryCore);
            uIInventoryCore.OpenInventory();
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<IInventoryCore>();
        }
    }
}