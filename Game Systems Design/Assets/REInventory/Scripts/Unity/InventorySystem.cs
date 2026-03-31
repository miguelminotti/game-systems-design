using MMStdLib.Utils;
using REInventory.Unity.UI;
using REInventory.Core;
using UnityEngine;

namespace REInventory.Unity
{
    public class InventorySystem : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private UIInventoryCore _uIInventoryCore;

        public IInventoryCore InventoryCore => _inventoryCore;

        private IInventoryCore _inventoryCore;

        private void Awake()
        {
            _inventoryCore = new InventoryCore();
            _inventoryCore.Initialize(_inventoryData);
            ServiceLocator.RegisterService(InventoryCore);
            _uIInventoryCore.OpenInventory();
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<IInventoryCore>();
        }
    }
}