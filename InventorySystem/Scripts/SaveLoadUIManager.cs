using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUIManager : MonoBehaviour
{
    public SaveLoadManager saveLoadManager;
    public Inventory inventory;
    public Equipment equipment;
    public QuickAccessBar quickAccessBar;

    public Button saveButton;
    public Button loadButton;

    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGame);
        }
    }

    public void SaveGame()
    {
        saveLoadManager.SaveGame(inventory, equipment, quickAccessBar);
    }

    public void LoadGame()
    {
        saveLoadManager.LoadGame(inventory, equipment, quickAccessBar);
    }
}