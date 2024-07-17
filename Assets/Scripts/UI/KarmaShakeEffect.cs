using PrimeTween;
using UnityEngine;

namespace UI
{
	public class KarmaShakeEffect : MonoBehaviour
	{
		public float Duration;
		public Vector3 PositionShake;
		public Vector3 RotationShake;
		
		private void Start()
		{
			GameManager.OnKarmaChange += OnKarmaChange;
		}
		
		private void OnDestroy()
		{
			GameManager.OnKarmaChange -= OnKarmaChange;
		}

		private void OnKarmaChange(int value, int delta)
		{
			if (delta < 0)
			{
				Tween.ShakeCamera(Camera.main, 2f);
				Tween.ShakeLocalPosition(transform, PositionShake, Duration);
				Tween.ShakeLocalRotation(transform, RotationShake, Duration);
			}
		}
	}
}