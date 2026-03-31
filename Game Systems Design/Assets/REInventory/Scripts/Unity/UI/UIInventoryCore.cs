using MMStdLib.Utils;
using UnityEngine;
using REInventory.Core;

namespace REInventory.Unity.UI
{
    public class UIInventoryCore : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private UIInventoryGridController _gridController;

        [ContextMenu("Open Inventory")]
        public void OpenInventory()
        {
            gameObject.SetActive(true);
            _gridController.Initialize();
            _gridController.BindInventory(ServiceLocator.GetService<IInventoryCore>());
            _gridController.DrawItems();
        }

        [ContextMenu("Close Inventory")]
        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }
    }
}