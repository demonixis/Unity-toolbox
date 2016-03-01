using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    public static class UIHelper
    {
        public static void SelectFirstButton(GameObject target, bool children)
        {
            Button btn = null;

            if (children)
                btn = target.GetComponentInChildren(typeof(Button)) as Button;
            else
                btn = target.GetComponent(typeof(Button)) as Button;

            SelectFirstButton(btn);
        }

        public static void SelectFirstButton(GameObject target)
        {
            SelectFirstButton(target, true);
        }

        public static void SelectFirstButton(Button button)
        {
            if (button != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        public static void ToggleUI()
        {
            var canvas = GetCanvas();
            if (canvas)
                canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        }

        public static Canvas GetCanvas()
        {
            var mainUI = GameObject.FindWithTag("MainUI");
            if (mainUI != null)
                return mainUI.GetComponent<Canvas>();

            return null;
        }

        public static void ScaleCanvas(Canvas canvas = null)
        {
            var width = 1280;
            var height = 800;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Level"))
            {
                width = 1024;
                height = 600;
            }

            ScaleCanvas(canvas, width, height);
        }

        public static void ScaleCanvas(Canvas canvas, int width, int height)
        {
            if (canvas == null)
                canvas = GetCanvas();

            var needUIScaling = false;

            if (Screen.width <= width || Screen.height <= height)
                needUIScaling = true;

#if UNITY_ANDROID
        needUIScaling = true;
#endif

            if (needUIScaling)
            {
                var canvasScaler = (CanvasScaler)canvas.GetComponent(typeof(CanvasScaler));
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(width, height);
            }
        }
    }
}