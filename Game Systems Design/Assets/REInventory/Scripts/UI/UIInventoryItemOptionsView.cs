using UnityEngine;

namespace REInventory.UI
{
    public class UIInventoryItemOptionsView : MonoBehaviour
    {
        [Header("Injections")]
        [SerializeField] private UIInventoryItemOption[] _options;

        private void Awake()
        {
            foreach (UIInventoryItemOption option in _options)
            {
                option.OnOptionClicked += OnOptionClicked;
            }
        }

        private void OnOptionClicked(UIInventoryItemOption option)
        {
            CloseOptions();
        }

        public void OpenOptions()
        {
            gameObject.SetActive(true);
        }

        public void CloseOptions()
        {
            gameObject.SetActive(false);
        }

        public void RefreshOptions(IStorableOption[] storableOptions)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                if (i < storableOptions.Length)
                {
                    _options[i].Refresh(storableOptions[i]);
                    _options[i].gameObject.SetActive(true);
                }
                else
                {
                    _options[i].gameObject.SetActive(false);
                }
            }
        }
    }
}