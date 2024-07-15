using System;
using System.Collections;
using QTE;
using TextManager;
using UnityEngine;

namespace TalkingHeads
{
	[Serializable]
	public class ChatItem
	{
		[Serializable]
		public class DialogueLineItem
		{
			[TextArea] public string Text;
			public bool IsOpponentSpeaks;
			public TextSpeed textSpeed;
			
			public CharacterSpriteContainer LeftSpeaker;
			public CharacterSpriteContainer RightSpeaker;
			
			[SerializeReference]
			public DialogueLineItem NextDialogueLineItem;

			public void PushNextDialogueLine(DialogueLineItem lineItem)
			{
				var currentItem = this;
				while (true)
				{
					if (currentItem.NextDialogueLineItem == null)
					{
						currentItem.NextDialogueLineItem = lineItem;
						break;
					}

					currentItem = currentItem.NextDialogueLineItem;
				}
			}
		}
			
		public DialogueLineItem Main;
		
		[Header("QTE")]
		public GameObject QTEPrefab;
		public int SuccessKarmaValue;
		public DialogueLineItem Success;
		public DialogueLineItem Fail;
	}

	public class ChatManager : MonoBehaviour
	{
		[SerializeField] private ChatItemsData _chatData;
		[SerializeField] private ChatItem[] _chatItems;

		[SerializeField] private CharacterTalkView _leftHead;
		[SerializeField] private CharacterTalkView _rightHead;

		[SerializeField] private TextTypeWriter _chatText;
		[SerializeField] private GameObject _nextDialogueMark;
		[SerializeField] private float _markFlickerDelay;

		[Header("Dialogue Timings")]
		[SerializeField]
		private float _startDialogueDelay = 1f;
		[SerializeField]
		private float _minDialogueDuration;

		[Header("QTE Settings")] 
		[SerializeField]
		private SpriteRenderer _qteFade;
		[SerializeField]
		private float _preQTEDelay;
		[SerializeField]
		private float _preQTEFadeDuraion;
		[SerializeField]
		private float _postQTEDelay;
		[SerializeField]
		private float _postQTEFadeDuraion;

		private ChatItem _currentChatItem;
		private ChatItem.DialogueLineItem _currentDialogueItem;
		private QTEHolder _currentQTE;
		private Coroutine _nextDialogueMarkCoroutine;

		public void StartChat()
		{
			StartCoroutine(PlayChat());
		}
		
		private IEnumerator PlayChat()
		{
			setInitialSprites();

			_nextDialogueMark.SetActive(false);
			_chatText.ClearText();

			yield return new WaitForSeconds(_startDialogueDelay);
			
			for (var i = 0; i < _chatData.ChatItems.Count; i++)
			{
				_currentChatItem = _chatData.ChatItems[i];
				
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
				yield return new WaitForSeconds(.5f);
				
				while (true)
				{
					_nextDialogueMark.SetActive(!_nextDialogueMark.activeSelf);

					yield return new WaitForSeconds(_markFlickerDelay);
				}
			}

			void ShowDialogueItem(ChatItem.DialogueLineItem item)
			{
				if (item.LeftSpeaker != null)
				{
					_leftHead.SetSpriteContainer(item.LeftSpeaker);
				}
				
				if (item.RightSpeaker != null)
				{
					_rightHead.SetSpriteContainer(item.RightSpeaker);
				}
				
				_currentDialogueItem = item;
				
				if (_currentDialogueItem.IsOpponentSpeaks == true)
				{
					_leftHead.SetActiveAndTalking();
					_rightHead.Deactivate();
				}
				else
				{
					_leftHead.Deactivate();
					_rightHead.SetActiveAndTalking();
				}
				
				_chatText.PlayText(_currentDialogueItem.Text, _currentDialogueItem.textSpeed);
			}

			IEnumerator processQTE(ChatItem currentChatItem)
			{
				yield return FadeHepler.FadeIn(_preQTEFadeDuraion, _qteFade, 0f, .7f);
				yield return new WaitForSeconds(_preQTEDelay);
				
				_currentQTE = Instantiate(currentChatItem.QTEPrefab).GetComponent<QTEHolder>();
				_currentQTE.SuccessKarmaDeltaValue = currentChatItem.SuccessKarmaValue;

				while (_currentQTE.IsComplete == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
				
				yield return FadeHepler.FadeOut(_postQTEFadeDuraion, _qteFade, .7f, 0f);
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
				
				if (_currentDialogueItem.IsOpponentSpeaks == true)
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

			void setInitialSprites()
			{
				if (_chatData.ChatItems[0].Main.LeftSpeaker != null)
				{
					_leftHead.SetSpriteContainer(_chatData.ChatItems[0].Main.LeftSpeaker);
					_leftHead.Deactivate();
				}
				else
				{
					_leftHead.SetEmpty();
				}

				if (_chatData.ChatItems[0].Main.RightSpeaker != null)
				{
					_rightHead.SetSpriteContainer(_chatData.ChatItems[0].Main.RightSpeaker);
					_rightHead.Deactivate();
				}
				else
				{
					_rightHead.SetEmpty();
				}
			}
		}
	}
}