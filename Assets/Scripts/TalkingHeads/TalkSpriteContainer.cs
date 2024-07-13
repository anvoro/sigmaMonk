using System.Collections.Generic;
using UnityEngine;

namespace TalkingHeads
{
	[CreateAssetMenu]
	public class TalkSpriteContainer : ScriptableObject
	{
		[SerializeField] private Sprite[] _talkSprites;

		public IReadOnlyList<Sprite> TalkSprites => _talkSprites;
	}
}