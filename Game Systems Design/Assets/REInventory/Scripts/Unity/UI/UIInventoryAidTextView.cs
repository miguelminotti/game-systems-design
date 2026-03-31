using MMStdLib.Utils;
using TMPro;
using UnityEngine;

namespace REInventory.Unity.UI
{
    public interface IInventoryAidTextChangeEvent
    {
        string NewText { get; set; }
    }

    public class InventoryAidTextChangeEvent : IInventoryAidTextChangeEvent
    {
        public string NewText { get; set; }
        public InventoryAidTextChangeEvent(string newText)
        {
            NewText = newText;
        }
    }

    public class UIInventoryAidTextView : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private TextMeshProUGUI _text;

        private void OnEnable()
        {
            GameEventBus.Subscribe<IInventoryAidTextChangeEvent>(OnInventoryAidTextChange);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<IInventoryAidTextChangeEvent>(OnInventoryAidTextChange);
        }

        private void OnInventoryAidTextChange(IInventoryAidTextChangeEvent evt)
        {
            _text.text = evt.NewText;
        }
    }
}