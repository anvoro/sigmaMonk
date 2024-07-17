using System;
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
	private static readonly IReadOnlyDictionary<TextSpeed, WaitForSeconds> delayCache = new Dictionary<TextSpeed, WaitForSeconds>
	{
		{ TextSpeed.Slow, new WaitForSeconds(.05f) },
		{ TextSpeed.Medium, new WaitForSeconds(.025f) },
		{ TextSpeed.Fast, new WaitForSeconds(.0125f) },
		{ TextSpeed.FF, new WaitForSeconds(.001f) },
	};
	
	private TMP_Text _text;

	public bool PlayComplete { get; private set; }

	public event Action OnPlayComplete;
	
	private void Awake()
	{
		PlayComplete = true;
		
		_text = GetComponent<TMP_Text>();
	}

	public void ClearText()
	{
		_text.text = string.Empty;
	}

	public void SetMaxSpeed()
	{
		_currentTextSpeed = TextSpeed.FF;
	}

	public void PlayText(string text, TextSpeed textSpeed)
	{
		StartCoroutine(ProcessText(text, textSpeed));
	}

	private TextSpeed _currentTextSpeed;
	private IEnumerator ProcessText(string text, TextSpeed textSpeed)
	{
		_currentTextSpeed = textSpeed;
		
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
			
			yield return delayCache[_currentTextSpeed];
		}
		
		OnPlayComplete?.Invoke();
		PlayComplete = true;
	}
}