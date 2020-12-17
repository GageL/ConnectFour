using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLucas;

namespace C4 {
	public class GameplayManager : Singleton<GameplayManager> {

		#region Constant Variables

		#endregion

		#region Static Variables
		public delegate void PlayerAction();
		public event PlayerAction OnAction;
		#endregion

		#region Public Variables

		#endregion

		#region Private Variables
		[Header("Runtime Debug")]
		public string PlayerName;
		public Color PlayerTeamColor;
		public Color AITeamColor;
		public bool IsPlayerTurn;
		public int ActionsTaken;
		public int CurrentRound;

		private Coroutine aiThinkProcess;
		#endregion

		#region Unity Methods
		private void Awake() {

		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void StartGame() {
			if (UnityEngine.Random.Range(0, 2) == 0) {
				IsPlayerTurn = true;
			} else {
				IsPlayerTurn = false;
			}
			GridManager.Instance.PrepareGrid();
			ActionsTaken = 0;
			CurrentRound = 1;
			if (IsPlayerTurn) {
				StartPlayerTurn();
			} else {
				StartAITurn();
			}
		}

		public void StartPlayerTurn() {
			GridManager.Instance.ShowTiles();
		}

		public void ReceiveTileSelection(GridTile gridTile) {
			GridManager.Instance.HideTiles();
		}

		public void EndPlayerTurn() {
			GridManager.Instance.HideTiles();
			LogTurnAction(true);
			StartAITurn();
		}

		public void StartAITurn() {
			if (aiThinkProcess != null) {
				StopCoroutine(aiThinkProcess);
				aiThinkProcess = null;
			}
			aiThinkProcess = StartCoroutine(AIThinkProcess());
		}

		public void EndAITurn() {
			LogTurnAction(false);
			StartPlayerTurn();
		}
		#endregion

		#region Local Methods
		private IEnumerator AIThinkProcess() {
			yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 2f)); //Make the user believe the "ai" is thinking
			GridTile[] gridTiles = GridManager.Instance.GetViableGridTiles();
			if (gridTiles != null && gridTiles.Length > 0) {
				GridTile selTile = gridTiles[UnityEngine.Random.Range(0, gridTiles.Length)];
				if (selTile != null) {
					selTile.SelectTile();
				}
			}
			yield return new WaitForSeconds(.5f);
			EndAITurn();
		}

		private void LogTurnAction(bool isPlayer) {
			IsPlayerTurn = !IsPlayerTurn;
			ActionsTaken++;
			if (ActionsTaken == 2) {
				CurrentRound++;
				ActionsTaken = 0;
			}
			OnAction?.Invoke();
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}