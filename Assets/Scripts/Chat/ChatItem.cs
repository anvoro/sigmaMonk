using System;
using TalkingHeads;
using UnityEngine;

namespace Chat
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
}