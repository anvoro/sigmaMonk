using System.Collections.Generic;
using UnityEngine;

namespace TalkingHeads
{
	[CreateAssetMenu]
	public class CharacterSpriteContainer : ScriptableObject
	{
		[SerializeField] private Sprite _silinceSprite;
		[SerializeField] private Sprite[] _talkSprites;

		public Sprite SilenceSprite => _silinceSprite;
		
		public IReadOnlyList<Sprite> TalkSprites => _talkSprites;
	}
}