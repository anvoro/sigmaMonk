using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using QTE;
using UnityEngine;
using UnityEngine.Serialization;

public class RingQTEView : MonoBehaviour
{
	private enum RingSize
	{
		Small = 0,
		Medium = 1,
		Large = 2
	}

	private enum RingProperty
	{
		Radius = 0,
		Width = 1,
		MaxGradientAlpha = 2,
		Color = 3
	}

	private static readonly IReadOnlyDictionary<RingSize, float> ringSizeCache = new Dictionary<RingSize, float>()
	{
		{ RingSize.Small, .1f },
		{ RingSize.Medium, .2f },
		{ RingSize.Large, .4f }
	};

	private static readonly IReadOnlyDictionary<RingSize, float> haloSizeCache = new Dictionary<RingSize, float>()
	{
		{ RingSize.Small, .15f },
		{ RingSize.Medium, .25f },
		{ RingSize.Large, .35f }
	};

	private static readonly Dictionary<RingProperty, int> ringPropertiesCache = new();

	[SerializeField] private MeshRenderer _dynamicRingRenderer;
	[SerializeField] private MeshRenderer _staticRingRenderer;
	[SerializeField] private MeshRenderer _outerHaloRenderer;
	[SerializeField] private MeshRenderer _innerHaloRenderer;

	[SerializeField] private RingSize _staticRingSize;
	[SerializeField] private RingSize _dynamicRingSize;

	[Header("Appearance Timing")] [SerializeField]
	private float _apperanceScale = .4f;

	[SerializeField] private float _apperanceScaleDelay = .2f;
	[SerializeField] private float _punchScale = .6f;
	[SerializeField] private Vector3 _punchForce;
	[SerializeField] private float _punchScaleDelay = .2f;

	[Header("Result Timing")] [SerializeField]
	private float _winPunchScale = .6f;

	[SerializeField] private Vector3 _winPunchForce;
	[SerializeField] private float _losePunchPosition = .6f;
	[SerializeField] private Vector3 _losePunch;
	[SerializeField] private float _flashTime = .05f;
	[SerializeField] private float _resultTime = 1f;

	[Header("Colors")] [SerializeField] private Color _winFlashColor;
	[SerializeField] private Color _winColor;
	[SerializeField] private Color _loseFlashColor;
	[SerializeField] private Color _loseColor;

	[Header("Ending Timing")] [SerializeField]
	private float _endingDelay = .2f;

	[SerializeField] private float _endingScale = .2f;

	private Material _staticRingMaterial;
	private Material _dynamicRingMaterial;
	private Material _outerHaloMaterial;
	private Material _innerHaloMaterial;

	private float _targetDynamicRadius;
	private float _initialDynamicRadius;

	public event Action OnQTEAnimationBegin;

	private void Start()
	{
		_staticRingMaterial = _staticRingRenderer.material;
		_dynamicRingMaterial = _dynamicRingRenderer.material;
		_outerHaloMaterial = _outerHaloRenderer.material;
		_innerHaloMaterial = _innerHaloRenderer.material;

		_targetDynamicRadius = ringSizeCache[_staticRingSize];
		_initialDynamicRadius = ringSizeCache[_dynamicRingSize];

		ActivateStaticRing(false);
		ActivateDynamicRing(false);
		ActivateOuterHalo(false);
		ActivateInnerHalo(false);

		initPropertiesCache();

		void initPropertiesCache()
		{
			ringPropertiesCache[RingProperty.Radius] = Shader.PropertyToID("_Radius");
			ringPropertiesCache[RingProperty.Width] = Shader.PropertyToID("_RingWidth");
			ringPropertiesCache[RingProperty.MaxGradientAlpha] = Shader.PropertyToID("_MaxGradAlpha");
			ringPropertiesCache[RingProperty.Color] = Shader.PropertyToID("_RingColor");
		}
	}

	public void Play(float qteTime)
	{
		StartCoroutine(PlayAppearanceAndBodyInternal(qteTime));
	}

	public void PlayResult(QTEResult result)
	{
		StartCoroutine(PlayResultInternal(result));
	}

	private void ActivateOuterHalo(bool isActive)
	{
		_outerHaloRenderer.gameObject.SetActive(isActive);
	}

	private void ActivateInnerHalo(bool isActive)
	{
		_innerHaloRenderer.gameObject.SetActive(isActive);
	}

	private void ActivateStaticRing(bool isActive)
	{
		_staticRingRenderer.gameObject.SetActive(isActive);
	}

	private void ActivateDynamicRing(bool isActive)
	{
		_dynamicRingRenderer.gameObject.SetActive(isActive);
	}

	private void SetFloatProperty(RingProperty ringProperty, Material ringMaterial, float value)
	{
		ringMaterial.SetFloat(ringPropertiesCache[ringProperty], value);
	}

	private float GetFloatProperty(RingProperty ringProperty, Material ringMaterial)
	{
		return ringMaterial.GetFloat(ringPropertiesCache[ringProperty]);
	}

	private Color GetColorProperty(Material ringMaterial)
	{
		return ringMaterial.GetColor(ringPropertiesCache[RingProperty.Color]);
	}

	private void SetColorProperty(Material ringMaterial, Color color)
	{
		ringMaterial.SetColor(ringPropertiesCache[RingProperty.Color], color);
	}

	private bool _isPlaying;

	private IEnumerator PlayAppearanceAndBodyInternal(float qteTime)
	{
		if (_isPlaying == true) throw new Exception($"{gameObject.name} already playing");

		_isPlaying = true;

		ActivateStaticRing(true);
		ActivateDynamicRing(true);

		Tween.Scale(transform, Vector3.zero, transform.localScale, _apperanceScale);
		yield return new WaitForSeconds(_apperanceScale + _apperanceScaleDelay);

		Tween.PunchScale(transform, _punchForce, _punchScale);
		yield return new WaitForSeconds(_punchScale + _punchScaleDelay);

		OnQTEAnimationBegin?.Invoke();

		var staticRingWidth = GetFloatProperty(RingProperty.Width, _staticRingMaterial);
		var dynamicRingWidth = GetFloatProperty(RingProperty.Width, _dynamicRingMaterial);

		var playTime = 0f;
		while (playTime < qteTime)
		{
			var t = playTime / qteTime;
			SetFloatProperty(RingProperty.Radius, _dynamicRingMaterial,
				Mathf.SmoothStep(_initialDynamicRadius, _targetDynamicRadius, t));
			SetFloatProperty(RingProperty.Width, _dynamicRingMaterial,
				Mathf.Lerp(dynamicRingWidth, staticRingWidth, t));
			SetFloatProperty(RingProperty.MaxGradientAlpha, _staticRingMaterial, Mathf.SmoothStep(0, .85f, t));

			playTime += Time.deltaTime;

			yield return GameManager.WaitEndOfFrame;
		}
	}

	private bool _isResultPlaying;

	private IEnumerator PlayResultInternal(QTEResult result)
	{
		if (_isResultPlaying == true) throw new Exception($"{gameObject.name} already playing ending");

		Color color;
		Color flashColor;
		switch (result)
		{
			case QTEResult.Success:
				color = _winColor;
				flashColor = _winFlashColor;
				Tween.PunchScale(transform, _winPunchForce, _winPunchScale);
				break;

			case QTEResult.LoseByInput:
			case QTEResult.LoseByTimer:
				color = _loseColor;
				flashColor = _loseFlashColor;
				Tween.ShakeLocalPosition(transform, _losePunch, _losePunchPosition);
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(result), result, null);
		}

		_isResultPlaying = true;

		ActivateDynamicRing(false);

		StartCoroutine(playInnerHalo());

		var startColor = GetColorProperty(_staticRingMaterial);

		yield return ProcessColorLerp(_flashTime, startColor, flashColor, _staticRingMaterial);
		yield return ProcessColorLerp(_flashTime, flashColor, color, _staticRingMaterial);

		ActivateOuterHalo(true);
		var initialHaloRadius = haloSizeCache[_staticRingSize];
		var endHaloRadius = initialHaloRadius * 2f;

		SetFloatProperty(RingProperty.Radius, _outerHaloMaterial, initialHaloRadius);
		SetColorProperty(_outerHaloMaterial, flashColor);

		yield return ProcessFloatLerp(_resultTime, initialHaloRadius, endHaloRadius, _outerHaloMaterial);

		StartCoroutine(PlayEnding());

		IEnumerator playInnerHalo()
		{
			ActivateInnerHalo(true);

			var startColor = GetColorProperty(_staticRingMaterial);

			yield return ProcessColorLerp(_flashTime, startColor, flashColor, _innerHaloMaterial);
			yield return ProcessColorLerp(_flashTime, flashColor, color, _innerHaloMaterial);

			ActivateInnerHalo(false);
		}
	}

	private IEnumerator PlayEnding()
	{
		var initialHaloRadius = GetFloatProperty(RingProperty.Radius, _outerHaloMaterial);
		StartCoroutine(ProcessFloatLerp(_endingScale * 2f, initialHaloRadius, 0, _outerHaloMaterial));
		yield return new WaitForSeconds(_endingDelay);

		Tween.Scale(transform, transform.localScale, Vector3.zero, _endingScale);
		yield return new WaitForSeconds(_endingScale);

		Destroy(gameObject);
	}

	private IEnumerator ProcessColorLerp(float processTime, Color a, Color b, Material material)
	{
		var currentTime = 0f;
		while (currentTime < processTime)
		{
			var t = currentTime / processTime;
			SetColorProperty(material, Color.Lerp(a, b, t));

			currentTime += Time.deltaTime;

			yield return GameManager.WaitEndOfFrame;
		}
	}

	private IEnumerator ProcessFloatLerp(float processTime, float a, float b, Material material)
	{
		var currentTime = 0f;
		while (currentTime < processTime)
		{
			var t = currentTime / processTime;
			SetFloatProperty(RingProperty.Radius, material, Mathf.SmoothStep(a, b, t));

			currentTime += Time.deltaTime;

			yield return GameManager.WaitEndOfFrame;
		}
	}
}