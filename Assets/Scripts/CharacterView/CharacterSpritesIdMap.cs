using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TalkingHeads
{
	[CreateAssetMenu]
	public class CharacterSpritesIdMap : ScriptableObject
	{
		[Serializable]
		private struct CharacterSpritesIdItem
		{
			public string Id;
			public CharacterSpriteContainer CharacterSpriteContainer;
		}

		[SerializeField]
		private List<CharacterSpritesIdItem> _items;

		public CharacterSpriteContainer GetSprite(string id)
		{
			return _items.First(e => e.Id == id).CharacterSpriteContainer;
		}
	}
}