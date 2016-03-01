using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
	public sealed class UISelectorWidget : MonoBehaviour
	{
		private int _index = 0;
		private int _size = 0;

		[SerializeField]
		private Text text = null;
		[SerializeField]
		private string[] options = null;
		[SerializeField]
		private bool interactable = true;

		public string[] Options
		{
			get { return options; }
			set
			{
				options = value;
				_size = options.Length;
				Index = 0;
			}
		}

		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				if (_index >= _size)
					_index = 0;
				else if (_index < 0)
					_index = _size - 1;

				UpdateText();
			}
		}

		public string Value
		{
			get { return options[_index]; }
			set { SetValueActive(value); }
		}

		void Awake()
		{
			if (_size == 0 && options != null)
				_size = options.Length;
		}

		void Start()
		{
			UpdateText();
		}

		public void SetInteractable(bool isInteractable)
		{
			var buttons = GetComponentsInChildren<Button>();
			for (int i = 0, l = buttons.Length; i < l; i++)
				buttons[i].interactable = isInteractable;

			interactable = isInteractable;
			enabled = interactable;
		}

		public void ChangeValue(bool inc)
		{
			if (interactable)
				Index += inc ? 1 : -1;
		}

		public void SetValueActive(string value)
		{
			var index = System.Array.IndexOf(options, value);
			if (index > -1)
				Index = index;
		}

		public void UpdateText()
		{
			if (text != null)
				text.text = options[_index];            
		}
	}
}