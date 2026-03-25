using UnityEngine;

namespace REInventory
{
    // Testing class
    [CreateAssetMenu(fileName = "New Storable Item", menuName = "RE Inventory/Storable Item")]
    public class StorableItem : ScriptableObject, IStorable
    {
        [Header("Settings")]
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int width;
        [SerializeField] private int height;

        public string Name => itemName; // TODO: add localisation support
        public string Description => description; // TODO: add localisation support
        public Sprite Icon => icon;
        public int Width => width;
        public int Height => height;

        public IRuntimeStorable GetRuntimeInstance()
        {
            return new RuntimeStorableItem(this);
        } 
    }
}