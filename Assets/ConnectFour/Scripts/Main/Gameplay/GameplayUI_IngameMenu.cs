using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using GLucas;
using TMPro;

namespace C4 {
	public class GameplayUI_IngameMenu : Singleton<GameplayUI_IngameMenu> {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables
		public GameObject RootHolder;
		#endregion

		#region Private Variables
		[SerializeField] private TMP_Text gameStatusText;
		[SerializeField] private Button pauseButton;
		[SerializeField] private TMP_Text roundText;
		[SerializeField] private TMP_Text turnText;
		[SerializeField] private GameObject pauseMenuHolder;
		[SerializeField] private Button pauseMenuResumeButton;
		[SerializeField] private Button pauseRestartButton;
		[SerializeField] private Button pauseMenuQuitButton;
		[SerializeField] private AudioMixer globalAudioMixer;
		[SerializeField] private Slider musicVolumeSlider;
		[SerializeField] private Slider effectsVolumeSlider;

		private Coroutine onGameStartProcess;
		private Coroutine onGameEndProcess;
		#endregion

		#region Unity Methods
		public override void Awake() {
			base.Awake();
			gameStatusText.color = new Color(gameStatusText.color.r, gameStatusText.color.g, gameStatusText.color.b, 0);
			roundText.text = $"Round: {GameplayManager.Instance.CurrentRound}";
			turnText.text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? "Player" : "Bot")}";
			pauseMenuHolder.SetActive(false);
			musicVolumeSlider.value = 1f;
			effectsVolumeSlider.value = 1f;
		}

		private void OnEnable() {
			pauseButton.onClick.AddListener(() => PauseButtonClick());
			pauseMenuResumeButton.onClick.AddListener(() => PauseMenuResumeButtonClick());
			pauseRestartButton.onClick.AddListener(() => PauseRestartButtonClick());
			pauseMenuQuitButton.onClick.AddListener(() => PauseMenuQuitButtonClick());

			GameplayManager.ON_GAME_START += OnGameStart;
			GameplayManager.ON_GAME_END += OnGameEnd;
		}

		private void OnDisable() {
			pauseButton.onClick.RemoveListener(() => PauseButtonClick());
			pauseMenuResumeButton.onClick.RemoveListener(() => PauseMenuResumeButtonClick());
			pauseRestartButton.onClick.RemoveListener(() => PauseRestartButtonClick());
			pauseMenuQuitButton.onClick.RemoveListener(() => PauseMenuQuitButtonClick());

			GameplayManager.ON_GAME_START -= OnGameStart;
			GameplayManager.ON_GAME_END -= OnGameEnd;
		}

		private void Update() {
			roundText.text = $"Round: {GameplayManager.Instance.CurrentRound}";
			roundText.transform.parent.GetComponent<TMP_Text>().text = $"Round: {GameplayManager.Instance.CurrentRound}";
			turnText.text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? GameplayManager.Instance.PlayerName : GameplayManager.Instance.BotName)}";
			turnText.transform.parent.GetComponent<TMP_Text>().text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? GameplayManager.Instance.PlayerName : GameplayManager.Instance.BotName)}";
		}
		#endregion

		#region Callback Methods
		private void OnGameStart() {
			if (onGameStartProcess != null) {
				StopCoroutine(onGameStartProcess);
				onGameStartProcess = null;
			}
			onGameStartProcess = StartCoroutine(OnGameStartProcess());
		}

		private void OnGameEnd() {
			if (onGameEndProcess != null) {
				StopCoroutine(onGameEndProcess);
				onGameEndProcess = null;
			}
			onGameEndProcess = StartCoroutine(OnGameEndProcess());
		}

		private void PauseButtonClick() {
			AudioManager.Instance.PlayButtonClick();
			Time.timeScale = 0; //Note this will cease the game coroutines as well unless switched to realtime yield
			pauseMenuHolder.SetActive(true);
		}

		private void PauseMenuResumeButtonClick() {
			AudioManager.Instance.PlayButtonClick();
			Time.timeScale = 1;
			pauseMenuHolder.SetActive(false);
		}

		private void PauseRestartButtonClick() {
			AudioManager.Instance.PlayButtonClick();
			Time.timeScale = 1;
			pauseMenuHolder.SetActive(false);
			GameplayManager.Instance.StartGame();
		}

		private void PauseMenuQuitButtonClick() {
			Application.Quit();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void SetMusicVolume(float value) {
			globalAudioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
		}

		public void SetEffectsVolume(float value) {
			globalAudioMixer.SetFloat("EffectsVolume", Mathf.Log10(value) * 20);
		}
		#endregion

		#region Local Methods
		private IEnumerator OnGameStartProcess() {
			gameStatusText.color = new Color(gameStatusText.color.r, gameStatusText.color.g, gameStatusText.color.b, 0);
			gameStatusText.text = "Match started, good luck!";
			float alpha = 0;
			while (alpha < 1f) {
				alpha += 1 * Time.deltaTime;
				gameStatusText.color = new Color(gameStatusText.color.r, gameStatusText.color.g, gameStatusText.color.b, alpha);
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(1f);
			while (alpha > 0) {
				alpha -= 0.5f * Time.deltaTime;
				gameStatusText.color = new Color(gameStatusText.color.r, gameStatusText.color.g, gameStatusText.color.b, alpha);
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator OnGameEndProcess() {
			if (!GameplayManager.Instance.WinStateFound) {
				gameStatusText.text = $"Stalemate!";
			} else {
				gameStatusText.text = $"Winner, {(GameplayManager.Instance.IsPlayerTurn ? GameplayManager.Instance.PlayerName : "Bot")}! <size={gameStatusText.fontSize / 3}><i>({GameplayManager.Instance.WinDirectionType.ToString()})</i></size>";
			}
			float alpha = 0;
			while (alpha < 1f) {
				alpha += 1 * Time.deltaTime;
				gameStatusText.color = new Color(gameStatusText.color.r, gameStatusText.color.g, gameStatusText.color.b, alpha);
				yield return new WaitForEndOfFrame();
			}
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}