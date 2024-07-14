using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class TextFlicker : MonoBehaviour
	{
		private TMP_Text _text;

		[SerializeField]
		private float _cycleDuration = .8f;
		
		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
		}

		private void Start()
		{
			StartCoroutine(ProcessFlicker());
		}

		private IEnumerator ProcessFlicker()
		{
			while (true)
			{
				yield return new WaitForSeconds(.6f);
				
				float currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_text.alpha = Mathf.SmoothStep(1, 0, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}

				yield return new WaitForSeconds(.2f);
			
				currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_text.alpha = Mathf.SmoothStep(0, 1, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}
			}
		}
	}
}