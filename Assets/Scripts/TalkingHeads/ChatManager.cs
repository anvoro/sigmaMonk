using System;
using System.Collections;
using QTE;
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
		
		[Serializable]
		public struct DialogueItem
		{
			[TextArea] public string Text;
			public TypeSpeed TypeSpeed;
			public TalkingHeadItem[] TalkingHeads;
		}

		public DialogueItem Main;
		
		[Header("QTE")]
		public GameObject QTEPrefab;
		public DialogueItem Success;
		public DialogueItem Fail;
	}

	public class ChatManager : MonoBehaviour
	{
		[SerializeField] private ChatItem[] _chatItems;

		[SerializeField] private CharacterTalkView _leftHead;
		[SerializeField] private CharacterTalkView _rightHead;

		[SerializeField] private TextTypeWriter _chatText;
		[SerializeField] private GameObject _nextDialogueMark;
		[SerializeField] private float _markFlickerDelay;

		[Header("Dialogue Timings")]
		[SerializeField]
		private float _minDialogueDuration;
		[SerializeField]
		private float _preQTEDelay;
		[SerializeField]
		private float _postQTEDelay;
		
		private QTEHolder _currentQTE;
		private Coroutine _nextDialogueMarkCoroutine;
		
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
		
		private IEnumerator PlayChat()
		{
			while (GameManager.I.HasInput == false)
			{
				yield return null;
			}
			
			for (var i = 0; i < _chatItems.Length; i++)
			{
				var currentChatItem = _chatItems[i];

				ShowDialogueItem(currentChatItem.Main);

				if (currentChatItem.QTEPrefab != null)
				{
					yield return processAwaitDialogue();
					yield return processQTE(currentChatItem);
				}
				else
				{
					yield return processAwaitDialogue();
				}
			}

			IEnumerator processNextDialogueMark()
			{
				while (true)
				{
					_nextDialogueMark.SetActive(!_nextDialogueMark.activeSelf);

					yield return new WaitForSeconds(_markFlickerDelay);
				}
			}

			void ShowDialogueItem(ChatItem.DialogueItem item)
			{
				_leftHead.SetSpritesContainer(null);
				_rightHead.SetSpritesContainer(null);
				
				foreach (var head in item.TalkingHeads)
				{
					SetHead(head);
				}

				_chatText.PlayText(item.Text, item.TypeSpeed);
			}

			IEnumerator processQTE(ChatItem currentChatItem)
			{
				yield return new WaitForSeconds(_preQTEDelay);
					
				_currentQTE = Instantiate(currentChatItem.QTEPrefab).GetComponent<QTEHolder>();

				while (_currentQTE.IsComplete == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
					
				yield return new WaitForSeconds(_postQTEDelay);

				ShowDialogueItem(_currentQTE.IsSuccessful() == true 
					? currentChatItem.Success 
					: currentChatItem.Fail);
				
				yield return processAwaitDialogue();
			}

			IEnumerator processAwaitDialogue()
			{
				yield return new WaitForSeconds(_minDialogueDuration);
				
				while (_chatText.PlayComplete == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
				
				_nextDialogueMarkCoroutine = StartCoroutine(processNextDialogueMark());
				
				while (GameManager.I.HasInput == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
					
				StopCoroutine(_nextDialogueMarkCoroutine);
				_nextDialogueMark.SetActive(false);
					
				yield return new WaitForSeconds(.1f);
			}
		}
	}
}