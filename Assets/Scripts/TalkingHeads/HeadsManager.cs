using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TalkingHeads
{
    public enum HeadPosition
    {
        Left = 0,
        Right = 1,
    }
    
    [Serializable]
    public struct ChatItem
    {
        [TextArea]
        public string Text;
        public HeadPosition HeadPosition;
        public Sprite Sprite;
    }
    
    public class HeadsManager : MonoBehaviour
    {
        [SerializeField]
        private ChatItem[] _chatItems;

        [SerializeField]
        private SpriteRenderer _leftHead;
        [SerializeField]
        private SpriteRenderer _rightHead;

        [SerializeField]
        private TextMeshProUGUI _chatText;

        public void SetHead(Sprite sprite, HeadPosition headPosition)
        {
            switch (headPosition)
            {
                case HeadPosition.Left:
                    _leftHead.sprite = sprite;
                    break;
                
                case HeadPosition.Right:
                    _rightHead.sprite = sprite;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(headPosition), headPosition, null);
            }
        }

        private void Start()
        {
            StartCoroutine(PlayChat());
        }

        private IEnumerator PlayChat()
        {
            for (int i = 0; i < _chatItems.Length; i++)
            {
                ChatItem currentItem = _chatItems[i];
                
                SetHead(currentItem.Sprite, currentItem.HeadPosition);

                _chatText.text = currentItem.Text;
                
                yield return new WaitForSeconds(1f);
            }
        }
    }
}