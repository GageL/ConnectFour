using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
		[SerializeField] private Button pauseButton;
		[SerializeField] private TMP_Text roundText;
		[SerializeField] private TMP_Text turnText;
		[SerializeField] private GameObject pauseMenuHolder;
		[SerializeField] private Button pauseMenuResumeButton;
		[SerializeField] private Button pauseMenuQuitButton;
		#endregion

		#region Unity Methods
		private void Awake() {
			roundText.text = $"Round: {GameplayManager.Instance.CurrentRound}";
			turnText.text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? "Player" : "Bot")}";
			pauseMenuHolder.SetActive(false);
		}

		private void OnEnable() {
			pauseButton.onClick.AddListener(() => PauseButtonClick());
			pauseMenuResumeButton.onClick.AddListener(() => PauseMenuResumeButtonClick());
			pauseMenuQuitButton.onClick.AddListener(() => PauseMenuQuitButtonClick());
		}

		private void OnDisable() {
			pauseButton.onClick.RemoveListener(() => PauseButtonClick());
			pauseMenuResumeButton.onClick.RemoveListener(() => PauseMenuResumeButtonClick());
			pauseMenuQuitButton.onClick.RemoveListener(() => PauseMenuQuitButtonClick());
		}

		private void Update() {
			roundText.text = $"Round: {GameplayManager.Instance.CurrentRound}";
			roundText.transform.parent.GetComponent<TMP_Text>().text = $"Round: {GameplayManager.Instance.CurrentRound}";
			turnText.text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? "Player" : "Bot")}";
			turnText.transform.parent.GetComponent<TMP_Text>().text = $"Turn: {(GameplayManager.Instance.IsPlayerTurn ? "Player" : "Bot")}";
		}
		#endregion

		#region Callback Methods
		private void PauseButtonClick() {
			if (pauseMenuHolder.activeSelf) { return; }
			pauseMenuHolder.SetActive(true);
		}

		private void PauseMenuResumeButtonClick() {
			pauseMenuHolder.SetActive(false);
		}

		private void PauseMenuQuitButtonClick() {
			Application.Quit();
		}

		private void EndTurnButtonClick() {
			GameplayManager.Instance.EndPlayerTurn();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Local Methods

		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}