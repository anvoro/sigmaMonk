﻿using System;
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
			public bool IsOpponentSpeaks;
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
		private ChatItem.DialogueItem _currentDialogueItem;
		private QTEHolder _currentQTE;
		private Coroutine _nextDialogueMarkCoroutine;

		public void StartChat()
		{
			StartCoroutine(PlayChat());
		}
		
		private IEnumerator PlayChat()
		{
			setInitialSprites();

			_chatText.ClearText();

			yield return new WaitForSeconds(_startDialogueDelay);
			
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
				yield return new WaitForSeconds(.5f);
				
				while (true)
				{
					_nextDialogueMark.SetActive(!_nextDialogueMark.activeSelf);

					yield return new WaitForSeconds(_markFlickerDelay);
				}
			}

			void ShowDialogueItem(ChatItem.DialogueItem item)
			{
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
				
				_chatText.PlayText(_currentDialogueItem.Text, _currentDialogueItem.TypeSpeed);
			}

			IEnumerator processQTE(ChatItem currentChatItem)
			{
				yield return FadeHepler.FadeIn(_preQTEFadeDuraion, _qteFade, 0f, .7f);
				yield return new WaitForSeconds(_preQTEDelay);
				
				_currentQTE = Instantiate(currentChatItem.QTEPrefab).GetComponent<QTEHolder>();

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
				if (_chatItems[0].LeftSpeaker != null)
				{
					_leftHead.SetSpriteContainer(_chatItems[0].LeftSpeaker);
					_leftHead.Deactivate();
				}
				else
				{
					_leftHead.SetEmpty();
				}

				if (_chatItems[0].RightSpeaker != null)
				{
					_rightHead.SetSpriteContainer(_chatItems[0].RightSpeaker);
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