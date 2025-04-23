using System;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public class ChangeCursorForVideoCapturing : MonoBehaviour
    {
        public Texture2D cursorTexture; 
        public Texture2D cursorTapTexture; 

        void Start()
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(cursorTapTexture, Vector2.zero, CursorMode.ForceSoftware);
            }
            if (Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
    }
}