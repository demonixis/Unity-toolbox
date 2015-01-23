using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

[RequireComponent(typeof(Text))]
public class TypeWritterEffect : MonoBehaviour
{
	private Text _text;
	private string _contentText;
	public float wordsPerSecond = 2;
	private float timeElapsed = 0;   

	void Start()
	{
		_text = GetComponent<Text>();
		_contentText = _text.text;
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;
		_text.text = GetWords(_contentText, (int)(timeElapsed * wordsPerSecond));
	}
	
	private string GetWords(string text, int wordCount)
	{
		int words = wordCount;

		for (int i = 0; i < text.Length; i++)
		{ 
			if (text[i] == ' ')
				words--;
			
			if (words <= 0)
				return text.Substring(0, i);
		}
		
		return text;
	}
}