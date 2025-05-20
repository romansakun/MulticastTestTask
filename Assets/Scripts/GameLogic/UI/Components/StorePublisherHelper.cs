// #if UNITY_EDITOR
// using UnityEngine;
//
// namespace GameLogic.UI.Components
// {
//     public class StorePublisherHelper : MonoBehaviour
//     {
//         [Header("Cursor")]
//         public bool useCursor = true;
//         public Texture2D cursorTexture; 
//         public Texture2D cursorTapTexture; 
//
//         void Start()
//         {
//             if (useCursor)
//             {
//                 Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
//             }
//         }
//
//         private void Update()
//         {
//             if (useCursor)
//             {
//                 if (Input.GetMouseButtonDown(0))
//                 {
//                     Cursor.SetCursor(cursorTapTexture, new Vector2(25, 0), CursorMode.ForceSoftware);
//                 }
//
//                 if (Input.GetMouseButtonUp(0))
//                 {
//                     Cursor.SetCursor(cursorTexture, new Vector2(25, 0), CursorMode.ForceSoftware);
//                 }
//             }
//
//             if (Input.GetKey(KeyCode.P))
//             {
//                 ScreenShot();
//             }
//         }
//
//         [ContextMenu("ScreenShot")]
//         private void ScreenShot()
//         {
//             ScreenCapture.CaptureScreenshot("screenshot.png");
//             Debug.Log("Скриншот сохранен!");
//         }
//         
//     }
// }
// #endif