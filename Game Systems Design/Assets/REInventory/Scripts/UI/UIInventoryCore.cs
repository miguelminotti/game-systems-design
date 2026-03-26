using UnityEngine;

namespace REInventory.UI
{
    public class UIInventoryCore : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private InventorySystem _inventorySystem;
        [SerializeField] private UIInventoryGridController _gridController;

        [ContextMenu("Open Inventory")]
        public void OpenInventory()
        {
            gameObject.SetActive(true);
            _gridController.Initialize();
            _gridController.BindInventory(_inventorySystem.InventoryCore); // TODO: change it for service locator later
            _gridController.DrawItems();
        }

        [ContextMenu("Close Inventory")]
        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }
    }
}