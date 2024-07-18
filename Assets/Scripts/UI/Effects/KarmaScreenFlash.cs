using System.Collections;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class KarmaScreenFlash : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;
		
		public float Duration;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			GameManager.OnKarmaChange += OnKarmaChange;
		}
		
		private void OnDestroy()
		{
			GameManager.OnKarmaChange -= OnKarmaChange;
		}

		private void OnKarmaChange(int value, int delta)
		{
			if (delta > 0)
			{
				StartCoroutine(ProcessFlash());
			}
		}

		private IEnumerator ProcessFlash()
		{
			yield return FadeHelper.FadeIn(Duration / 2f, _canvasGroup);
			yield return FadeHelper.FadeOut(Duration / 2f, _canvasGroup);
		}
	}
}