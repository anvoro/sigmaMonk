using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace QTE
{
	public class QTEHolder : MonoBehaviour
	{
		[SerializeField]
		private RingQTEModel[] _qtes;
		
		[SerializeField]
		private float[] _apperanceDelay;

		private RingQTEModel _currentQte;
		
		public bool IsComplete { get; private set; }

		public bool IsSuccessful()
		{
			if (IsComplete == false)
			{
				throw new InvalidOperationException("Not all QTEs is completed");
			}
			
			return _qtes.All(e => e.Result == QTEResult.Success);
		}
		
		private void Start()
		{
			StartCoroutine(StartQTEsWithDelay());
		}

		private void Update()
		{
			if (IsComplete == true)
			{
				return;
			}
			
			for (int i = 0; i < _qtes.Length; i++)
			{
				var qte = _qtes[i];
				if (qte.IsProcessing == true)
				{
					if (GameManager.I.HasInput == true)
					{
						qte.SetInput();
					}
					
					break;
				}
			}
			
			int completeCounter = 0;
			for (int i = 0; i < _qtes.Length; i++)
			{
				var qte = _qtes[i];
				
				if (qte.IsComplete == true)
				{
					completeCounter++;
				}
			}
			
			if (completeCounter == _qtes.Length)
			{
				IsComplete = true;
				GameManager.I.ChangeKarma(IsSuccessful() == true ? 1 : -1);
			}
		}

		private IEnumerator StartQTEsWithDelay()
		{
			for (int i = 0; i < _qtes.Length; i++)
			{
				yield return new WaitForSeconds(_apperanceDelay[i]);
				
				_qtes[i].ShowQTE();
			}
		}
	}
}