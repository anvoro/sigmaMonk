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
			GameManager.OnKarmaChange += (k, d) =>
			{
				foreach (var item in _items)
				{
					if (item.KarmaToShow == k)
					{
						_image.sprite = item.Sprite;
						break;
					}
				}
			};
		}
	}
}