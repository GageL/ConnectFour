using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace C4 {
	[RequireComponent(typeof(Button))]
	public class GridTile : MonoBehaviour {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables

		private Button localButton;

		[Header("Runtime Debug")]
		public bool IsPopulated;
		public bool IsPlayerOwned;
		#endregion

		#region Unity Methods
		private void Awake() {
			localButton = this.GetComponent<Button>();
		}

		private void OnEnable() {
			localButton.onClick.AddListener(() => LocalButtonClicked());
		}

		private void OnDisable() {
			localButton.onClick.RemoveListener(() => LocalButtonClicked());
		}

		private void Start() {
			HidePlaceable();
		}
		#endregion

		#region Callback Methods
		private void LocalButtonClicked() {
			SelectTile();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void ShowPlaceable() {
			this.transform.GetChild(0).gameObject.SetActive(true);
		}

		public void HidePlaceable() {
			this.transform.GetChild(0).gameObject.SetActive(false);
		}

		public void SelectTile() {
			IsPopulated = true;
			IsPlayerOwned = GameplayManager.Instance.IsPlayerTurn;

			this.transform.GetChild(0).gameObject.SetActive(true);
			this.transform.GetChild(0).gameObject.GetComponent<Image>().color = (IsPlayerOwned ? GameplayManager.Instance.PlayerTeamColor : GameplayManager.Instance.AITeamColor);
			this.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

			GameplayManager.Instance.ReceiveTileSelection(this);

			localButton.interactable = false;
		}

		public void ResetTile() {
			HidePlaceable();

			IsPopulated = false;
			IsPlayerOwned = false;

			this.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.green;
			this.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

			localButton.interactable = true;
		}
		#endregion

		#region Local Methods

		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}