using System.Collections;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class StartGameUI : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private GameObject[] _objectsToHideOnFade;
		[SerializeField]
		private float _fadeDuration = 3f;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			StartCoroutine(AwaitPlayerInput());
		}

		private IEnumerator AwaitPlayerInput()
		{
			while (GameManager.I.HasInput == false)
			{
				yield return null;
			}

			foreach (var go in _objectsToHideOnFade)
			{
				go.SetActive(false);
			}
			
			GameManager.I.StartNewGame();

			yield return FadeHepler.FadeOut(_fadeDuration, _canvasGroup);
			
			Destroy(gameObject);
		}
	}
}