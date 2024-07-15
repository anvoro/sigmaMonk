using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum TextSpeed
{
	Slow = 0,
	Medium = 1,
	Fast = 2,
	FF = 3,
}

[RequireComponent(typeof(TMP_Text))]
public class TextTypeWriter : MonoBehaviour
{
	private static readonly IReadOnlyDictionary<TextSpeed, float> delayCache = new Dictionary<TextSpeed, float>
	{
		{ TextSpeed.Slow, .1f },
		{ TextSpeed.Medium, .05f },
		{ TextSpeed.Fast, .025f },
		{ TextSpeed.FF, .005f },
	};
	
	private TMP_Text _text;

	public bool PlayComplete { get; private set; }
	
	private void Awake()
	{
		_text = GetComponent<TMP_Text>();
	}

	public void ClearText()
	{
		_text.text = string.Empty;
	}

	public void PlayText(string text, TextSpeed textSpeed)
	{
		StartCoroutine(ProcessText(text, textSpeed));
	}

	private IEnumerator ProcessText(string text, TextSpeed textSpeed)
	{
		PlayComplete = false;

		ClearText();
		
		for (var i = 0; i < text.Length; i++)
		{
			if (text[i] == '<')
			{
				while (text[i] != '>')
				{
					_text.text += text[i];
					i++;
				}
			}

			_text.text += text[i];
			
			yield return new WaitForSeconds(delayCache[textSpeed]);
		}
		
		PlayComplete = true;
	}
}