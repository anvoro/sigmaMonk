using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class ImageFlicker : MonoBehaviour
	{
		private enum FlickerType
		{
			Fade = 0,
			Switch = 1,
		}
		
		private Image _image;

		[SerializeField]
		private FlickerType _flickerType;
		
		[SerializeField]
		private float _cycleDuration = .8f;
		[SerializeField]
		private float _newCycleDelay = .4f;
		
		private Color _initialColor;
		private Color _0alphaColor;
		
		private void Awake()
		{
			_image = GetComponent<Image>();
			
			_initialColor = _image.color;
			
			_0alphaColor = _initialColor;
			_0alphaColor.a = 0f;
		}

		private void OnEnable()
		{
			_image.color = _initialColor;

			switch (_flickerType)
			{
				case FlickerType.Fade:
					StartCoroutine(ProcessFade());
					break;
				
				case FlickerType.Switch:
					StartCoroutine(processSwitch());
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
		
		IEnumerator processSwitch()
		{
			yield return new WaitForSeconds(_newCycleDelay);
				
			while (true)
			{
				if (_image.color.a > Mathf.Epsilon)
				{
					_image.color = _0alphaColor;
				}
				else
				{
					_image.color = _initialColor;
				}

				yield return new WaitForSeconds(_cycleDuration);
			}
		}

		private IEnumerator ProcessFade()
		{
			while (true)
			{
				yield return new WaitForSeconds(_newCycleDelay);
				
				float currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_image.color = Color.Lerp(_initialColor, _0alphaColor, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}

				yield return new WaitForSeconds(_newCycleDelay / 2.5f);
			
				currentTime = 0f;
				while (currentTime < _cycleDuration)
				{
					var t = currentTime / _cycleDuration;
					_image.color = Color.Lerp(_0alphaColor, _initialColor, t);

					currentTime += Time.deltaTime;

					yield return GameManager.WaitEndOfFrame;
				}
			}
		}
	}
}