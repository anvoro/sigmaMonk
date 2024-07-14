using System;
using System.Collections;
using QTE;
using UnityEngine;

namespace TalkingHeads
{
	[Serializable]
	public struct ChatItem
	{
		[Serializable]
		public struct DialogueItem
		{
			[TextArea] public string Text;
			public bool IsPlayerSpeaks;
			public TypeSpeed TypeSpeed;
		}
		
		public CharacterSpriteContainer LeftSpeaker;
		public CharacterSpriteContainer RightSpeaker;

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

		private ChatItem _currentChatItem;
		private ChatItem.DialogueItem _currentDialogueItem;
		private QTEHolder _currentQTE;
		private Coroutine _nextDialogueMarkCoroutine;

		private void Start()
		{
			StartCoroutine(PlayChat());
		}
		
		private IEnumerator PlayChat()
		{
			_leftHead.SetEmpty();
			_rightHead.SetEmpty();
			
			while (GameManager.I.HasInput == false)
			{
				yield return null;
			}
			
			for (var i = 0; i < _chatItems.Length; i++)
			{
				_currentChatItem = _chatItems[i];

				if (_currentChatItem.LeftSpeaker != null)
				{
					_leftHead.SetSpriteContainer(_currentChatItem.LeftSpeaker);
				}
				
				if (_currentChatItem.RightSpeaker != null)
				{
					_rightHead.SetSpriteContainer(_currentChatItem.RightSpeaker);
				}
				
				ShowDialogueItem(_currentChatItem.Main);

				if (_currentChatItem.QTEPrefab != null)
				{
					yield return processAwaitDialogue();
					yield return processQTE(_currentChatItem);
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
				_currentDialogueItem = item;
				
				if (_currentDialogueItem.IsPlayerSpeaks == true)
				{
					_leftHead.SetActiveAndTalking();
					_rightHead.Deactivate();
				}
				else
				{
					_leftHead.Deactivate();
					_rightHead.SetActiveAndTalking();
				}
				
				_chatText.PlayText(_currentDialogueItem.Text, _currentDialogueItem.TypeSpeed);
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
				
				if (_currentDialogueItem.IsPlayerSpeaks == true)
				{
					_leftHead.StopTalking();
				}
				else
				{
					_rightHead.StopTalking();
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