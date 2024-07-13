using System;
using System.Collections;
using UnityEngine;

namespace QTE
{
	public enum QTEResult
	{
		Success = 0,
		LoseByInput = 1,
		LoseByTimer = 2
	}

	[RequireComponent(typeof(RingQTEView))]
	public class RingQTEModel : MonoBehaviour
	{
		private RingQTEView _ringView;

		[SerializeField] private float _qteTime = 1.4f;
		[SerializeField] private float _qteSuccessOffsetBefore = .2f;
		[SerializeField] private float _qteSuccessOffsetAfter = .2f;

		private void Awake()
		{
			_ringView = GetComponent<RingQTEView>();
			_ringView.OnQTEAnimationBegin += () => StartCoroutine(ProcessQTEInput());
		}

		// private IEnumerator Start()
		// {
		//     yield return new WaitForSeconds(2f);
		//     
		//     ShowQTE();
		// }

		public void ShowQTE()
		{
			_ringView.Play(_qteTime);
		}

		private IEnumerator ProcessQTEInput()
		{
			var maxQteTime = _qteTime + _qteSuccessOffsetAfter;
			var failQteTime = _qteTime - _qteSuccessOffsetBefore;
			float currentQteTime = 0;

			while (currentQteTime < maxQteTime)
			{
				if (GameManager.I.HasInput == true)
				{
					if (currentQteTime < failQteTime)
					{
						_ringView.PlayResult(QTEResult.LoseByInput);
						Debug.Log("[QTE: FAIL]");
					}
					else
					{
						_ringView.PlayResult(QTEResult.Success);
						Debug.Log("[QTE: SUCCESS]");
					}

					yield break;
				}

				currentQteTime += Time.deltaTime;

				yield return GameManager.WaitEndOfFrame;
			}

			_ringView.PlayResult(QTEResult.LoseByTimer);
			Debug.Log("[QTE: FAIL BY TIMER]");
		}
	}
}