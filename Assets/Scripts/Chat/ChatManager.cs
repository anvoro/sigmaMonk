using System.Collections;
using Chat;
using QTE;
using TextManager;
using UnityEngine;

namespace TalkingHeads
{
	public class ChatManager : MonoBehaviour
	{
		[SerializeField] private ChatItemsData _chatData;
		[SerializeField] private ChatItem[] _chatItems;

		[SerializeField] private CharacterTalkView _leftHead;
		[SerializeField] private CharacterTalkView _rightHead;

		[SerializeField] private TextTypeWriter _chatText;
		[SerializeField] private GameObject _nextDialogueMark;

		[SerializeField] private CanvasGroup _chatCanvas;
		
		[Header("Dialogue Timings")]
		[SerializeField]
		private float _startDialogueDelay = 1f;
		[SerializeField]
		private float _afterSkipDelay = .4f;

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

		public void StartChat()
		{
			StartCoroutine(PlayChat());
		}
		
		private IEnumerator PlayChat()
		{
			setInitialState();
			
			yield return FadeHelper.FadeIn(_startDialogueDelay, _chatCanvas);
			
			for (var i = 0; i < _chatData.ChatItems.Count; i++)
			{
				_currentChatItem = _chatData.ChatItems[i];
				
				yield return ShowDialogueItemRecursively(_currentChatItem.Main, true);

				if (_currentChatItem.QTEPrefab != null)
				{
					yield return processQTE(_currentChatItem);
				}
			}
			
			yield return GameManager.I.EndGame();

			IEnumerator ShowDialogueItemRecursively(ChatItem.DialogueLineItem item, bool isMainBranch)
			{
				var currentLineItem = item;
				
				while (currentLineItem != null)
				{
					if (currentLineItem.LeftSpeaker != null)
					{
						_leftHead.SetSpriteContainer(currentLineItem.LeftSpeaker);
					}
				
					if (currentLineItem.RightSpeaker != null)
					{
						_rightHead.SetSpriteContainer(currentLineItem.RightSpeaker);
					}
				
					_currentDialogueItem = currentLineItem;
				
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
				
					yield return processAwaitDialogue();

					currentLineItem = currentLineItem.NextDialogueLineItem;
				}
			}

			IEnumerator processQTE(ChatItem currentChatItem)
			{
				yield return FadeHelper.FadeIn(_preQTEFadeDuraion, _qteFade);
				yield return new WaitForSeconds(_preQTEDelay);
				
				_currentQTE = Instantiate(currentChatItem.QTEPrefab).GetComponent<QTEHolder>();
				_currentQTE.SuccessKarmaDeltaValue = currentChatItem.SuccessKarmaValue;

				while (_currentQTE.IsComplete == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
				
				yield return FadeHelper.FadeOut(_postQTEFadeDuraion, _qteFade);
				yield return new WaitForSeconds(_postQTEDelay);

				if (currentChatItem.Success == null && currentChatItem.Fail == null)
				{
					yield break;
				}
				
				yield return ShowDialogueItemRecursively(_currentQTE.IsSuccessful() == true 
					? currentChatItem.Success
					: currentChatItem.Fail
					,false);
			}

			IEnumerator processAwaitDialogue()
			{
				bool showTextImmediately = false;
				while (_chatText.PlayComplete == false)
				{
					if (GameManager.I.HasInput == true)
					{
						_chatText.SetMaxSpeed();
						showTextImmediately = true;
						break;
					}
					
					yield return GameManager.WaitEndOfFrame;
				}

				if (showTextImmediately == true)
				{
					yield return new WaitForSeconds(_afterSkipDelay);
				}
				
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

				_nextDialogueMark.SetActive(true);
				
				while (GameManager.I.HasInput == false)
				{
					yield return GameManager.WaitEndOfFrame;
				}
				
				_nextDialogueMark.SetActive(false);
					
				yield return new WaitForSeconds(.1f);
			}

			void setInitialState()
			{
				var fadeColor = _qteFade.color;
				fadeColor.a = 0;
				_qteFade.color = fadeColor;
				
				_chatCanvas.alpha = 0f;
				_nextDialogueMark.SetActive(false);
				_chatText.ClearText();
				
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