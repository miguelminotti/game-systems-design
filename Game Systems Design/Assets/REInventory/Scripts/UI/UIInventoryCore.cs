using UnityEngine;

namespace REInventory.UI
{
    public class UIInventoryCore : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private InventorySystem _inventorySystem;
        [SerializeField] private UIInventoryGridController _gridController;

        private void OnEnable()
        {
            _gridController.BindInventory(_inventorySystem.InventoryCore); // TODO: change it for service locator later
            _gridController.DrawItems();
        }

        [ContextMenu("Open Inventory")]
        public void OpenInventory()
        {
            gameObject.SetActive(true);
        }

        [ContextMenu("Close Inventory")]
        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }
    }
}