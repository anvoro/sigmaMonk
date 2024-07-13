using UnityEngine;
using System.Collections;
using TMPro;
using yutokun;

public class UITextTypeWriter : MonoBehaviour
{
	public TMP_Text _tmpText;
	private string story;
	public float delay;

	private void Start()
	{
		_tmpText = GetComponent<TMP_Text>();
		_tmpText.text = "";
		var fabula = CSVParser.LoadFromPath("/plot.csv");
	}

	private void Awake()
	{
		story = _tmpText.text;
		_tmpText.text = "";
		StartCoroutine(PlayText());
	}

	private IEnumerator PlayText()
	{
		for (var i = 0; i < story.Length; i++)
		{
			if (story[i] == '<')
			{
				while (story[i] != '>')
				{
					_tmpText.text += story[i];
					i++;
				}
			}

			_tmpText.text += story[i];
			
			yield return new WaitForSeconds(delay);
		}
	}
}