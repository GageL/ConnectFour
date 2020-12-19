using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLucas;
using TMPro;
using UnityEngine.EventSystems;

namespace C4 {
	public class GameplayUI_PregameMenu : Singleton<GameplayUI_PregameMenu> {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables
		public GameObject RootHolder;
		#endregion

		#region Private Variables
		[SerializeField] private TMP_InputField localPlayerNameInput;
		[SerializeField] private Transform playerColorButtonsHolder;
		[SerializeField] private Image playerSelectedColorImage;
		[SerializeField] private Transform aiColorButtonsHolder;
		[SerializeField] private Image aiSelectedColorImage;
		[SerializeField] private TMP_Text errorText;
		[SerializeField] private Button startGameButton;

		[Header("Runtime Debug")]
		public int selPlayerColorIdx;
		public int selAIColorIdx;
		#endregion

		#region Unity Methods
		private void Awake() {
			localPlayerNameInput.text = "Long John Shivver";
			string[] botNames = {
				"Doris Shutt",
				"Stan Dupp",
				"Yullbe Allwright",
				"Kay Oss",
				"Levy Tate",
				"Maxi Mum",
				"Rhoda Camel",
				"Ivana Fly",
				"Holli Wood",
				"Joe King"
			};
			string botName = botNames[UnityEngine.Random.Range(0, 10)];
			GameplayManager.Instance.BotName = botName;

			playerSelectedColorImage.color = playerColorButtonsHolder.GetChild(selPlayerColorIdx).GetComponent<Image>().color;
			aiSelectedColorImage.color = playerColorButtonsHolder.GetChild(selAIColorIdx).GetComponent<Image>().color;
			errorText.text = string.Empty;
		}

		private void OnEnable() {
			startGameButton.onClick.AddListener(() => StartGameButtonClick());
		}

		private void OnDisable() {
			startGameButton.onClick.RemoveListener(() => StartGameButtonClick());
		}

		private void Start() {
			AudioManager.Instance.PlayLobbyTrack();
		}
		#endregion

		#region Callback Methods
		public void PlayerColorButtonClick(int idx) {
			selPlayerColorIdx = idx;
			playerSelectedColorImage.color = playerColorButtonsHolder.GetChild(selPlayerColorIdx).GetComponent<Image>().color;
		}

		public void AIColorButtonClick(int idx) {
			selAIColorIdx = idx;
			aiSelectedColorImage.color = playerColorButtonsHolder.GetChild(selAIColorIdx).GetComponent<Image>().color;
		}

		private void StartGameButtonClick() {
			AudioManager.Instance.PlayButtonClick();
			if (string.IsNullOrEmpty(localPlayerNameInput.text) || string.IsNullOrWhiteSpace(localPlayerNameInput.text)) {
				errorText.text = "Cannot start until the local player is named";
				return;
			}
			if (selPlayerColorIdx == selAIColorIdx) {
				errorText.text = "The player and bot cannot have the same color";
				return;
			}
			errorText.text = string.Empty;
			RootHolder.SetActive(false);
			GameplayManager.Instance.PlayerName = localPlayerNameInput.text;
			GameplayManager.Instance.PlayerTeamColor = playerColorButtonsHolder.GetChild(selPlayerColorIdx).GetComponent<Image>().color;
			GameplayManager.Instance.AITeamColor = aiColorButtonsHolder.GetChild(selAIColorIdx).GetComponent<Image>().color;
			GameplayUI_IngameMenu.Instance.RootHolder.SetActive(true);
			GameplayManager.Instance.StartGame();
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