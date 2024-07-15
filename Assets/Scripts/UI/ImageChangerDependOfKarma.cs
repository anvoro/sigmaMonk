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
			public int KarmaToShow;
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
			foreach (var item in _items)
			{
				if (item.KarmaToShow == value)
				{
					_image.sprite = item.Sprite;
					break;
				}
			}
		}
	}
}