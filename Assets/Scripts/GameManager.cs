using Core;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
	public static readonly WaitForEndOfFrame WaitEndOfFrame = new();

	//public event Action OnPlayerInput;

	[SerializeField] private float _inputDelay = 0.05f;

	private float _timeSinceLastInput;

	public bool HasInput { get; private set; }

	private void Update()
	{
		_timeSinceLastInput += Time.deltaTime;

		if (_timeSinceLastInput > _inputDelay
		    && Input.GetKeyDown(KeyCode.Space) == true)
		{
			_timeSinceLastInput = 0f;
			//OnPlayerInput?.Invoke();

			HasInput = true;

			//Debug.Log("OnPlayerInput");
		}
		else
		{
			HasInput = false;
		}
	}
}