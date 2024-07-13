using System.Collections;
using UnityEngine;

namespace QTE
{
	public enum QTEResult
	{
		None = 0,
		Success = 1,
		LoseByInput = 2,
		LoseByTimer = 3,
	}

	[RequireComponent(typeof(RingQTEView))]
	public class RingQTEModel : MonoBehaviour
	{
		private RingQTEView _ringView;

		[SerializeField] private float _qteTime = 1.4f;
		[SerializeField] private float _qteSuccessOffsetBefore = .2f;
		[SerializeField] private float _qteSuccessOffsetAfter = .2f;

		public bool IsProcessing { get; private set; }
		public bool IsComplete { get; private set; }
		
		public QTEResult Result { get; private set; } = QTEResult.None;
		
		private void Awake()
		{
			_ringView = GetComponent<RingQTEView>();
			_ringView.OnQTEAnimationBegin += () => StartCoroutine(ProcessQTEInput());
		}

		public void ShowQTE()
		{
			IsProcessing = false;
			IsComplete = false;
			
			_ringView.Play(_qteTime);
		}

		public void SetInput()
		{
			_hasInput = true;
		}

		private bool _hasInput = false;
		private IEnumerator ProcessQTEInput()
		{
			IsProcessing = true;
			
			var maxQteTime = _qteTime + _qteSuccessOffsetAfter;
			var failQteTime = _qteTime - _qteSuccessOffsetBefore;
			float currentQteTime = 0;

			while (currentQteTime < maxQteTime)
			{
				if (_hasInput == true)
				{
					if (currentQteTime < failQteTime)
					{
						Result = QTEResult.LoseByInput;
					}
					else
					{
						Result = QTEResult.Success;
					}

					IsComplete = true;
					IsProcessing = false;
					_ringView.PlayResult(Result);

					yield break;
				}

				currentQteTime += Time.deltaTime;

				yield return GameManager.WaitEndOfFrame;
			}
			
			IsComplete = true;
			IsProcessing = false;
			
			Result = QTEResult.LoseByTimer;
			_ringView.PlayResult(Result);
		}
	}
}