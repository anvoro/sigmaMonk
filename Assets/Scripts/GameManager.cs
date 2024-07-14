using System;
using Core;
using TalkingHeads;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : SingletonBase<GameManager>
{
	public static readonly WaitForEndOfFrame WaitEndOfFrame = new();

	public static event Action<int> OnKarmaChange;
	
	[SerializeField] private float _inputDelay = 0.05f;

	[SerializeField] private ChatManager _chatManager;
	
	private float _timeSinceLastInput;
	private int _karma = 0;

	public bool HasInput { get; private set; }

	public void ChangeKarma(int value)
	{
		_karma += value;
		
		OnKarmaChange?.Invoke(_karma);
	}

	public void StartGame()
	{
		_chatManager.StartChat();
	}
	
	private void Update()
	{
		_timeSinceLastInput += Time.deltaTime;

		if (_timeSinceLastInput > _inputDelay
		    && Input.GetKeyDown(KeyCode.Space) == true)
		{
			_timeSinceLastInput = 0f;

			HasInput = true;
		}
		else
		{
			HasInput = false;
		}
	}
}