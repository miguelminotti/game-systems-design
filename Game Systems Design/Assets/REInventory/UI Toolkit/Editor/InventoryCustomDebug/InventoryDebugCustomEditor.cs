using MMStdLib.Utils;
using REInventory.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryDebugCustomEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private TextInputBaseField<int> _xPositionInputField;
    private TextInputBaseField<int> _yPositionInputField;
    private ObjectField _storableObjectField;
    private const string _placeItemButtonName = "PlaceItemButton";

    [MenuItem("Tools/Inventory Debug")]
    public static void ShowWindow()
    {
        InventoryDebugCustomEditor wnd = GetWindow<InventoryDebugCustomEditor>();
        wnd.titleContent = new GUIContent("InventoryDebugCustomEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        SetupInputFields();
        SetupButtonHandler();
    }

    private void SetupInputFields()
    {
        VisualElement root = rootVisualElement;

        _xPositionInputField = root.Q<TextInputBaseField<int>>("XPosition");
        _yPositionInputField = root.Q<TextInputBaseField<int>>("YPosition");
        _storableObjectField = root.Q<ObjectField>();
        _storableObjectField.objectType = typeof(IStorable);
    }

    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var button = root.Q<Button>(_placeItemButtonName);
        if (button != null)
        {
            RegisterButtonHandler(button);
        } else
        {
            Debug.Log("Button not found");
        }
    }

    private void RegisterButtonHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(OnButtonClickedCallback);
    }

    private void OnButtonClickedCallback(ClickEvent @event)
    {
        int xPosition = System.Convert.ToInt16(_xPositionInputField.text);
        int yPosition = System.Convert.ToInt16(_yPositionInputField.text);

        var inventoryService = ServiceLocator.GetService<IInventoryCore>();
        if (inventoryService != null && _storableObjectField.value is IStorable storableItem)
        {
            PlaceItem(storableItem, inventoryService, xPosition, yPosition);
        } else
        {
            Debug.Log("Inventory not found");
        }
    }

    private void PlaceItem(IStorable storable, IInventoryCore inventory, int x, int y)
    {
        if (storable == null)
        {
            Debug.LogWarning("No item assigned.");
            return;
        }

        IStorable newItem = storable;
        IRuntimeStorable runtimeItem = newItem.GetRuntimeInstance();

        if (inventory.AddItemAtPosition(runtimeItem, new GridPosition(x, y)) == IInventoryGrid.PlaceItemResult.Succeeded)
        {
            Debug.Log($"Item placed in inventory.");
        }
        else
        {
            Debug.Log($"Failed to place item.");
        }
    }
}
