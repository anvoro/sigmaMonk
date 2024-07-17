using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class EndGameUI : MonoBehaviour
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private Sprite _goodEnd;
		[SerializeField]
		private Sprite _badEnd;

		private void Start()
		{
			_image.gameObject.SetActive(false);
		}

		public void ShowEnding()
		{
			if (GameManager.I.KarmaValue > 0)
			{
				_image.sprite = _goodEnd;
			}
			else
			{
				_image.sprite = _badEnd;
			}
		}
	}
}