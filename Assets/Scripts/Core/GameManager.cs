using System;
using System.Collections;
using Core;
using TalkingHeads;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	public static readonly WaitForEndOfFrame WaitEndOfFrame = new();

	public static event Action<int, int> OnKarmaChange;
	
	[SerializeField] private float _inputDelay = 0.05f;
	[SerializeField] private ChatManager _chatManager;
	[SerializeField] private GameObject _menu;
	[SerializeField] private EndGameUI _endGameUI;
	[SerializeField] private CanvasGroup _fade;
	[SerializeField] private float _finalFadeDuration;
	
	private float _timeSinceLastInput;
	private int _karma = 0;

	private bool _inMenuState;

	public bool HasInput { get; private set; }

	public int KarmaValue => _karma;

	private void Start()
	{
		_menu.SetActive(false);
	}

	public IEnumerator EndGame()
	{
		yield return FadeHepler.FadeIn(_finalFadeDuration, _fade);
		
		_endGameUI.ShowEnding();
		
		yield return FadeHepler.FadeOut(_finalFadeDuration, _fade);
	}
	
	public void ChangeKarma(int value)
	{
		_karma += value;
		
		OnKarmaChange?.Invoke(_karma, value);
	}

	public void StartNewGame()
	{
		_chatManager.StartChat();
	}

	private void ResetGameState()
	{
		HasInput = false;
		_karma = 0;
		_timeSinceLastInput = 0f;
		_inMenuState = false;
		
		Time.timeScale = 1f;
		
		_menu.SetActive(false);
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_inMenuState == false)
			{
				_inMenuState = true;
				_menu.SetActive(true);
				Time.timeScale = 0f;
			}
			else
			{
				_inMenuState = false;
				_menu.SetActive(false);
				Time.timeScale = 1f;
			}
		}

		if (_inMenuState == false)
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
		else
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				SceneManager.LoadScene(0);
				ResetGameState();
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				Application.Quit();
			}
		}
	}
}