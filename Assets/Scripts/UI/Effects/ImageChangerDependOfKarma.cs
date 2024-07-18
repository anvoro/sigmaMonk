using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class ImageChangerDependOfKarma : MonoBehaviour
	{
		[Serializable]
		private struct KarmaImageItem
		{
			public Sprite Sprite;
			public int MaxKarmaToShow;
			public int MinKarmaToShow;
		}

		private Image _image;

		[SerializeField] private KarmaImageItem[] _items;

		private void Awake()
		{
			_image = GetComponent<Image>();
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
			Sprite result = null;
			foreach (var item in _items)
			{
				if (value >= item.MinKarmaToShow && value <= item.MaxKarmaToShow)
				{
					_image.sprite = item.Sprite;
					break;
				}
			}
		}
	}
}