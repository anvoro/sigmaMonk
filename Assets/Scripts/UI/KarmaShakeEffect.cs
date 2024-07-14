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
			GameManager.OnKarmaChange += (k, d) =>
			{
				if (d < 0)
				{
					Tween.ShakeCamera(Camera.main, 2f);
					Tween.ShakeLocalPosition(transform, PositionShake, Duration);
					Tween.ShakeLocalRotation(transform, RotationShake, Duration);
				}
			};
		}
	}
}