using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class ImageFlicker : MonoBehaviour
	{
		private Image _image;

		[SerializeField]
		private float _cycleDuration = .8f;
		
		private void Awake()
		{
			_image = GetComponent<Image>();
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

				Color a1 = _image.color;
				Color a0 = a1;
				a0.a = 0;
				
				float currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_image.color = Color.Lerp(a1, a0, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}

				yield return new WaitForSeconds(.2f);
			
				currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_image.color = Color.Lerp(a0, a1, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}
			}
		}
	}
}