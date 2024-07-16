using System;
using System.Collections.Generic;
using System.Linq;
using QTE;
using TalkingHeads;
using TextManager;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace yutokun
{
	public class TextImporter : MonoBehaviour
	{
		[SerializeField]
		private QTEIdMap _qteMap;
		
		[SerializeField]
		private CharacterSpritesIdMap _spritesMap;

		public ChatItemsData ChatItemsData;

		public string FilePath = "C:\\Users\\user\\Downloads\\Malenkiy plot - Лист1.csv";
		
		private const int CHARACTER_ID = 0;
		private const int TEXT = 1;
		private const int SPRITE_ID1 = 2;
		private const int SPRITE_ID2 = 3;
		private const int QTE_ID = 4;
		private const int TEXT_SPEED = 5;
		private const int BRANCH_TYPE = 6;
		private const int KARMA_VALUE = 7;
		
		private enum DialogueBranchType
		{
			Main = 0,
			Success = 1,
			Fail = 2,
		}

		private DialogueBranchType _currentBranchType;
		private TextSpeed _currentTextSpeed;
		private bool _currentSpeakerIsOpponent;
		
		public void ImportText()
		{
			var csv = CSVParser.LoadFromPath(FilePath);
			csv = csv.Skip(1).ToList();
			
			var result = ChatItemsData.ChatItems;
			result.Clear();
			
			for (var rowIndex = 0; rowIndex < csv.Count; rowIndex++)
			{
				var row = csv[rowIndex];
				ChatItem.DialogueLineItem currentDialogueLine = new();

				processBranchType(row, rowIndex, currentDialogueLine);
				
				processTextSpeed(row, currentDialogueLine);
				processText(row, currentDialogueLine);
				processChatacterId(row, currentDialogueLine);
				processChatacterSprites(row, SPRITE_ID1, currentDialogueLine);
				processChatacterSprites(row, SPRITE_ID2, currentDialogueLine);
				processQTE(row, result[^1]);
				processKarma(row, result[^1]);
			}

#if UNITY_EDITOR
			EditorUtility.SetDirty(this.ChatItemsData);
#endif
			
			Debug.Log("Text Import: COMPLETE");
			
			void processBranchType(List<string> row, int rowIndex, ChatItem.DialogueLineItem currentDialogueLine)
			{
				switch (row[BRANCH_TYPE])
				{
					case "main":
						result.Add(new ChatItem());
						_currentBranchType = DialogueBranchType.Main;
						result[^1].Main = currentDialogueLine;
						break;
					
					case "success":
						_currentBranchType = DialogueBranchType.Success;
						result[^1].Success = currentDialogueLine;
						break;
					
					case "fail":
						_currentBranchType = DialogueBranchType.Fail;
						result[^1].Fail = currentDialogueLine;
						break;
					
					default:
						switch (_currentBranchType)
						{
							case DialogueBranchType.Main:
								result[^1].Main.PushNextDialogueLine(currentDialogueLine);
								break;
							
							case DialogueBranchType.Success:
								result[^1].Success.PushNextDialogueLine(currentDialogueLine);
								break;
							
							case DialogueBranchType.Fail:
								result[^1].Fail.PushNextDialogueLine(currentDialogueLine);
								break;
							
							default:
								throw new ArgumentOutOfRangeException();
						}
						break;
				}
			}

			void processTextSpeed(List<string> row, ChatItem.DialogueLineItem currentDialogueLine)
			{
				switch (row[TEXT_SPEED])
				{
					case "Slow":
						currentDialogueLine.textSpeed = TextSpeed.Slow;
						_currentTextSpeed = TextSpeed.Slow;
						break;

					case "Medium":
						currentDialogueLine.textSpeed = TextSpeed.Medium;
						_currentTextSpeed = TextSpeed.Medium;
						break;

					case "Fast":
						currentDialogueLine.textSpeed = TextSpeed.Fast;
						_currentTextSpeed = TextSpeed.Fast;
						break;
						
					case "FF":
						currentDialogueLine.textSpeed = TextSpeed.FF;
						_currentTextSpeed = TextSpeed.FF;
						break;

					default:
						currentDialogueLine.textSpeed = _currentTextSpeed;
						break;
				}
			}
			
			void processText(List<string> row, ChatItem.DialogueLineItem currentDialogueLine)
			{
				string lineHeight = "<line-height=100%>";
				
				switch (row[TEXT])
				{
					case null:
					case "":
						currentDialogueLine.Text = lineHeight + "...";
						break;

					default:
						currentDialogueLine.Text = lineHeight + row[TEXT];
						break;
				}
			}
			
			void processQTE(List<string> row, ChatItem currentChatItem)
			{
				switch (row[QTE_ID])
				{
					case null:
					case "":
						break;

					default:
						currentChatItem.QTEPrefab = _qteMap.GetQTE(row[QTE_ID]);
						break;
				}
			}
			
			void processChatacterId(List<string> row, ChatItem.DialogueLineItem currentDialogueLine)
			{
				switch (row[CHARACTER_ID])
				{
					case "0":
						currentDialogueLine.IsOpponentSpeaks = true;
						_currentSpeakerIsOpponent = true;
						break;
					
					case "1":
						currentDialogueLine.IsOpponentSpeaks = false;
						_currentSpeakerIsOpponent = false;
						break;
					
					default:
						currentDialogueLine.IsOpponentSpeaks = _currentSpeakerIsOpponent;
						break;
				}
			}
			
			void processChatacterSprites(List<string> row, int columnId, ChatItem.DialogueLineItem currentDialogueLine)
			{
				switch (row[columnId])
				{
					case null:
					case "":
						break;

					default:
						if (columnId == SPRITE_ID1)
						{
							currentDialogueLine.LeftSpeaker = _spritesMap.GetSprite(row[columnId]);
						}
						else
						{
							currentDialogueLine.RightSpeaker = _spritesMap.GetSprite(row[columnId]);
						}
						break;
				}
			}
			
			void processKarma(List<string> row, ChatItem currentChatItem)
			{
				switch (row[KARMA_VALUE])
				{
					case null:
					case "":
						break;

					default:
						currentChatItem.SuccessKarmaValue = int.Parse(row[KARMA_VALUE]);
						break;
				}
			}
		}
	}
}