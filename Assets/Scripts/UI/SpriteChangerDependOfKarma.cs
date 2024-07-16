using System;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteChangerDependOfKarma : MonoBehaviour
	{
		[Serializable]
		private struct KarmaImageItem
		{
			public Sprite Sprite;
			public int KarmaToShow;
		}

		private SpriteRenderer _sprite;

		[SerializeField] private KarmaImageItem[] _items;

		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
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
			foreach (var item in _items)
			{
				if (item.KarmaToShow == value)
				{
					Debug.Log(value);
					
					_sprite.sprite = item.Sprite;
					break;
				}
			}
		}
	}
}