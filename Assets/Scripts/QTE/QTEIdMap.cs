using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QTE
{
	[CreateAssetMenu]
	public class QTEIdMap : ScriptableObject
	{
		[Serializable]
		private struct QTEIdItem
		{
			public string Id;
			public GameObject QTEPrefab;
		}

		[SerializeField]
		private List<QTEIdItem> _items;

		public GameObject GetQTE(string id)
		{
			return _items.First(e => e.Id == id).QTEPrefab;
		}
	}
}