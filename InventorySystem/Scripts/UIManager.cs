using System.Collections.Generic;
using UnityEngine;

namespace Nakshatra.InventorySystem
{
    public class UIManager : MonoBehaviour
    {
        [System.Serializable]
        public class CanvasInfo
        {
            public string name;
            public GameObject canvas;
        }

        [Header("Inventory Panel Toggles")]
        [Tooltip("Key to open inventory panels")]  
        public KeyCode openKey = KeyCode.I;
        [Tooltip("Key to close inventory panels")]  
        public KeyCode closeKey = KeyCode.Escape;

        [Header("Time Control")]
        [Tooltip("Freeze game time when inventory is opened")]
        public bool freezeTimeOnOpen = false;

        public List<CanvasInfo> canvases = new List<CanvasInfo>();

        private bool isOpen = false;
        private float previousTimeScale = 1f;

        void Start()
        {
            // Ensure cursor is unlocked on start, then hide inventory
            UnlockCursor();
            DisableCanvases();
        }

        void Update()
        {
            // Open inventories
            if (!isOpen && Input.GetKeyDown(openKey))
            {
                EnableCanvases();
                isOpen = true;
            }
            // Close inventories
            else if (isOpen && Input.GetKeyDown(closeKey))
            {
                DisableCanvases();
                isOpen = false;
            }
        }

        public void EnableCanvases()
        {
            foreach (var canvasInfo in canvases)
            {
                if (canvasInfo.canvas != null)
                    canvasInfo.canvas.SetActive(true);
            }
            UnlockCursor();

            if (freezeTimeOnOpen)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }

        public void DisableCanvases()
        {
            foreach (var canvasInfo in canvases)
            {
                if (canvasInfo.canvas != null)
                    canvasInfo.canvas.SetActive(false);
            }
            LockCursor();

            if (freezeTimeOnOpen)
            {
                Time.timeScale = previousTimeScale;
            }
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
    }
}