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
			GameManager.OnKarmaChange += (k, d) =>
			{
				if (d > 0)
				{
					StartCoroutine(ProcessFlash());
				}
			};
		}

		private IEnumerator ProcessFlash()
		{
			yield return FadeHepler.FadeIn(Duration / 2f, _canvasGroup);
			yield return FadeHepler.FadeOut(Duration / 2f, _canvasGroup);
		}
	}
}