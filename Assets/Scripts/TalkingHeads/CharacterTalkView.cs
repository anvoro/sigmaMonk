using System.Collections;
using UnityEngine;

namespace TalkingHeads
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class CharacterTalkView : MonoBehaviour
	{
		private SpriteRenderer _sr;

		private TalkSpriteContainer _spriteContainer;
		private Coroutine _coroutine;

		[SerializeField] private float _spriteChangeDelay;

		private void Awake()
		{
			_sr = GetComponent<SpriteRenderer>();
		}

		public void SetSpritesContainer(TalkSpriteContainer spriteContainer)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
			
			_spriteContainer = spriteContainer;
			
			if (_spriteContainer == null)
			{
				_sr.sprite = null;
			}
			else
			{
				if (_spriteContainer.TalkSprites.Count > 1)
					_coroutine = StartCoroutine(ProcessTalkSprites());
				else
					_sr.sprite = _spriteContainer.TalkSprites[0];
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

				yield return new WaitForSeconds(_spriteChangeDelay);
			}
		}
	}
}