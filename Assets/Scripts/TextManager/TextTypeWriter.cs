using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum TypeSpeed
{
	Slow = 0,
	Medium = 1,
	Fast = 2,
	FF = 3,
}

[RequireComponent(typeof(TMP_Text))]
public class TextTypeWriter : MonoBehaviour
{
	private static readonly IReadOnlyDictionary<TypeSpeed, float> delayCache = new Dictionary<TypeSpeed, float>
	{
		{ TypeSpeed.Slow, .1f },
		{ TypeSpeed.Medium, .05f },
		{ TypeSpeed.Fast, .025f },
		{ TypeSpeed.FF, .005f },
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

	public void PlayText(string text, TypeSpeed typeSpeed)
	{
		StartCoroutine(ProcessText(text, typeSpeed));
	}

	private IEnumerator ProcessText(string text, TypeSpeed typeSpeed)
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
			
			yield return new WaitForSeconds(delayCache[typeSpeed]);
		}
		
		PlayComplete = true;
	}
}