using System;
using System.Collections;
using UnityEngine;

namespace TalkingHeads
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class CharacterTalkView : MonoBehaviour
	{
		private enum CharacterFadeType
		{
			None = 0,
			Faded = 1,
			Active = 2,
		}
		
		private SpriteRenderer _sr;

		private CharacterSpriteContainer _spriteContainer;
		private Coroutine _talkCoroutine;

		private CharacterFadeType _fadeType;

		[SerializeField] private float _talkSpriteChangeDelay = .5f;
		
		[Header("Fade")]
		[SerializeField] private float _fadeDuration = .2f;
		[SerializeField] private Color _fadeSpriteColor = Color.gray;
		
		private void Awake()
		{
			_sr = GetComponent<SpriteRenderer>();
		}

		public void SetSpriteContainer(CharacterSpriteContainer spriteContainer)
		{
			if (spriteContainer == null)
			{
				throw new NullReferenceException(nameof(spriteContainer));
			}
			
			if (spriteContainer.TalkSprites.Count <= 1)
			{
				throw new ArgumentOutOfRangeException($"{nameof(spriteContainer.TalkSprites.Count)} must be > 1");
			}
			
			_spriteContainer = spriteContainer;
		}
		
		public void SetEmpty()
		{
			StopTalkCoroutine();
			
			_spriteContainer = null;
			_sr.sprite = null;
		}
		
		public void SetActiveAndTalking()
		{
			StopTalkCoroutine();

			StartCoroutine(SetFade(false));
			
			_talkCoroutine = StartCoroutine(ProcessTalkSprites());
		}
		
		private void StopTalkCoroutine()
		{
			if (_talkCoroutine != null)
			{
				StopCoroutine(_talkCoroutine);
				_talkCoroutine = null;
			}
		}

		public void StopTalking()
		{
			StopTalkCoroutine();
			
			if (_spriteContainer != null)
			{
				_sr.sprite = _spriteContainer.SilenceSprite;
			}
		}

		public void Deactivate()
		{
			StopTalking();
			
			StartCoroutine(SetFade(true));
		}

		private IEnumerator SetFade(bool fadeIn)
		{
			Color initColor;
			Color endColor;
			CharacterFadeType fadeType;
			
			if (fadeIn == true)
			{
				initColor = Color.white;
				endColor = _fadeSpriteColor;
				fadeType = CharacterFadeType.Faded;
			}
			else
			{
				initColor = _fadeSpriteColor;
				endColor = Color.white;
				fadeType = CharacterFadeType.Active;
			}

			if (_fadeType == fadeType)
			{
				yield break;
			}

			_fadeType = fadeType;
			
			var currentTime = 0f;
			while (currentTime < _fadeDuration)
			{
				var t = currentTime / _fadeDuration;
				_sr.color = Color.Lerp(initColor, endColor, t);

				currentTime += Time.deltaTime;

				yield return GameManager.WaitEndOfFrame;
			}
		}

		private IEnumerator ProcessTalkSprites()
		{
			var spriteIndex = 0;
			while (true)
			{
				_sr.sprite = _spriteContainer.TalkSprites[spriteIndex];
				spriteIndex++;

				if (spriteIndex >= _spriteContainer.TalkSprites.Count) spriteIndex = 0;

				yield return new WaitForSeconds(_talkSpriteChangeDelay);
			}
		}
	}
}