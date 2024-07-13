using System;
using System.Collections;
using QTE;
using TMPro;
using UnityEngine;

namespace TalkingHeads
{
	public enum HeadPosition
	{
		Left = 0,
		Right = 1
	}

	[Serializable]
	public struct ChatItem
	{
		[Serializable]
		public struct TalkingHeadItem
		{
			public HeadPosition HeadPosition;
			public TalkSpriteContainer SpriteContainer;
		}

		[TextArea] public string Text;
		public float Duration;
		public TalkingHeadItem[] TalkingHeads;
		public GameObject QTEPrefab;
	}

	public class ChatManager : MonoBehaviour
	{
		[SerializeField] private ChatItem[] _chatItems;

		[SerializeField] private CharacterTalkView _leftHead;
		[SerializeField] private CharacterTalkView _rightHead;

		[SerializeField] private TextMeshProUGUI _chatText;
		
		private void SetHead(ChatItem.TalkingHeadItem headItem)
		{
			switch (headItem.HeadPosition)
			{
				case HeadPosition.Left:
					_leftHead.SetSpritesContainer(headItem.SpriteContainer);
					break;

				case HeadPosition.Right:
					_rightHead.SetSpritesContainer(headItem.SpriteContainer);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(HeadPosition), headItem.HeadPosition, null);
			}
		}

		private void Start()
		{
			StartCoroutine(PlayChat());
		}

		private QTEHolder _currentQTE;
		private IEnumerator PlayChat()
		{
			for (var i = 0; i < _chatItems.Length; i++)
			{
				var currentItem = _chatItems[i];

				foreach (var head in currentItem.TalkingHeads)
				{
					SetHead(head);
				}

				_chatText.text = currentItem.Text;

				if (currentItem.QTEPrefab != null)
				{
					_currentQTE = Instantiate(currentItem.QTEPrefab).GetComponent<QTEHolder>();

					while (_currentQTE.IsComplete == false)
					{
						yield return GameManager.WaitEndOfFrame;
					}
					
					yield return new WaitForSeconds(currentItem.Duration);
				}
				else
				{
					while (GameManager.I.HasInput == false)
					{
						yield return GameManager.WaitEndOfFrame;
					}
					
					yield return GameManager.WaitEndOfFrame;
				}
			}
		}
	}
}