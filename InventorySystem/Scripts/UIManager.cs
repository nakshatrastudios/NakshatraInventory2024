using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class CanvasInfo
    {
        public string name;
        public GameObject canvas;
    }

    public List<CanvasInfo> canvases = new List<CanvasInfo>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.I))
        {
            EnableCanvases();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableCanvases();
        }
    }

    public void EnableCanvases()
    {
        foreach (var canvasInfo in canvases)
        {
            canvasInfo.canvas.SetActive(true);
        }
        UnlockCursor();
    }

    public void DisableCanvases()
    {
        foreach (var canvasInfo in canvases)
        {
            canvasInfo.canvas.SetActive(false);
        }
        LockCursor();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        UnlockCursor();
        DisableCanvases();
    }
}
