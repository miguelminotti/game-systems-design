using MMStdLib.Utils;
using UnityEditor;
using UnityEngine;

namespace REInventory.Editor
{
    public class InventoryDebugWindow : EditorWindow
    {
        private StorableItem _item;
        private int _xPosition = 0;
        private int _yPosition = 0;

        private IInventoryCore _inventory;

        [MenuItem("Tools/Inventory Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<InventoryDebugWindow>("Inventory Debug");
        }

        private void OnGUI()
        {
            GUILayout.Label("Place Item", EditorStyles.boldLabel);

            _item = (StorableItem)EditorGUILayout.ObjectField("Item", _item, typeof(StorableItem), false);
            _xPosition = EditorGUILayout.IntField("X Position", _xPosition);
            _yPosition = EditorGUILayout.IntField("Y Position", _yPosition);

            GUILayout.Space(10);

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Place Item"))
            {
                PlaceItem();
            }

            GUI.enabled = true;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to use this tool.", MessageType.Info);
            }
        }

        private void PlaceItem()
        {
            if (_item == null)
            {
                Debug.LogWarning("No item assigned.");
                return;
            }

            if (_inventory == null)
            {
                _inventory = ServiceLocator.GetService<IInventoryCore>();

                if (_inventory == null)
                {
                    Debug.LogError("Inventory service not found.");
                    return;
                }
            }

            IStorable newItem = _item;
            IRuntimeStorable runtimeItem = newItem.GetRuntimeInstance();

            if (_inventory.AddItemAtPosition(runtimeItem, _xPosition, _yPosition))
            {
                Debug.Log($"Item placed in inventory.");
            }
            else
            {
                Debug.Log($"Failed to place item.");
            }
        }
    }
}