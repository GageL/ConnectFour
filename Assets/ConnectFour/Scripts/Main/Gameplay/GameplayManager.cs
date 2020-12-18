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
		public GameObject PlacementTile;
		#endregion

		#region Private Variables
		[Header("Runtime Debug")]
		public string PlayerName;
		public Color PlayerTeamColor;
		public Color AITeamColor;
		public bool IsPlayerTurn;
		public int ActionsTaken;
		public int CurrentRound;
		public bool IsTileDropping;
		public GridTile DestinationTile;

		private Coroutine aiThinkProcess;
		#endregion

		#region Unity Methods
		private void Awake() {

		}

		private void Update() {
			if (IsTileDropping) {
				PlacementTile.transform.GetChild(0).GetComponent<Image>().color = IsPlayerTurn ? PlayerTeamColor : AITeamColor;
				PlacementTile.SetActive(true);
				if (IsTileDropping) {
					PlacementTile.transform.position = Vector3.MoveTowards(PlacementTile.transform.position, DestinationTile.transform.position, 1.5f);
					if (Vector3.Distance(PlacementTile.transform.position, DestinationTile.transform.position) <= 0.1f) {
						DestinationTile.transform.GetChild(0).gameObject.SetActive(true);
						AnalyzeBoardState();
						IsTileDropping = false;
						DestinationTile = null;
					}
				}
			} else {
				PlacementTile.SetActive(false);
			}
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
			PlacementTile.transform.position = GridManager.Instance.LanePlacements[gridTile.AssignedLane].transform.position;
			DestinationTile = gridTile;
			IsTileDropping = true;
			GridManager.Instance.HideTiles();
		}

		public void EndPlayerTurn() {
			GridManager.Instance.HideTiles();
			LogTurnAction();
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
			LogTurnAction();
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
		}

		private void GetMatchingOrRandomTile() {

		}

		private void LogTurnAction() {
			IsPlayerTurn = !IsPlayerTurn;
			ActionsTaken++;
			if (ActionsTaken == 2) {
				CurrentRound++;
				ActionsTaken = 0;
			}
			OnAction?.Invoke();
		}

		private void AnalyzeBoardState() {
			Debug.Log("AnalyzeBoardState");
			bool foundWinScanario = false;

			//Vertical
			int verticalMatches = 0;
			if (!foundWinScanario) {
				for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
					for (int tile = GridManager.Instance.gridLanes[lane].gridTiles.Length; tile-- > 0;) { //Bottom->Top
						if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated) {
							if (IsPlayerTurn) {
								if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
									verticalMatches++;
									if (verticalMatches == 4) {
										foundWinScanario = true;
										break;
									}
								} else {
									verticalMatches = 0;
								}
							}
						}
					}
					if (foundWinScanario) {
						break;
					}
				}
			}

			//Horizontal

			//Diagonal
			//https://stackoverflow.com/questions/32770321/connect-4-check-for-a-win-algorithm

			if (foundWinScanario) {
				SetWinState();
			} else {
				Debug.Log("All losers");
				if (DestinationTile.IsPlayerOwned) {
					EndPlayerTurn();
				} else {
					EndAITurn();
				}
			}
		}

		private void SetWinState() {
			Debug.Log("Winner winner");
			if (IsPlayerTurn) {

			} else {

			}
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}