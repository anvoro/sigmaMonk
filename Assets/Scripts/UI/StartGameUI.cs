using System.Collections;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class StartGameUI : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private GameObject _text;
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
			
			_text.gameObject.SetActive(false);
			
			GameManager.I.StartGame();

			yield return FadeHepler.FadeOut(_fadeDuration, _canvasGroup);
			
			Destroy(gameObject);
		}
	}
}