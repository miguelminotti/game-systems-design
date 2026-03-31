using UnityEngine;
using REInventory.Core;

namespace REInventory.Unity
{
    [CreateAssetMenu(menuName = "RE Inventory/Inventory Data")]
    public class InventoryData : ScriptableObject, IInventoryData
    {
        [Header("Settings")]
        [SerializeField] private int width;
        [SerializeField] private int height;

        public int Width => width;
        public int Height => height;
    }
}