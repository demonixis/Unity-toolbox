#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.Editor
{
	public class FontReplacerEditor : EditorWindow
	{
		private Font _replacer;

		[MenuItem("Demonixis/Font Replacer")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(FontReplacerEditor));
		}

		void OnGUI()
		{
			GUILayout.Label("Desired Font", EditorStyles.boldLabel);

			_replacer = EditorGUILayout.ObjectField("Font", _replacer, typeof(Font), true) as Font;

			if (GUILayout.Button("Replace"))
			{
				var fonts = Editor.FindObjectsOfType<Text>();
				foreach (var font in fonts)
					font.font = _replacer;

				Debug.Log(fonts.Length + " fonts replaced");
			}
		}    
	}
}
#endif