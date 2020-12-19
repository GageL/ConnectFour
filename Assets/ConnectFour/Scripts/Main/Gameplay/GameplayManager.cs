using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLucas;
using C4.Utilities;

namespace C4 {
	public class GameplayManager : Singleton<GameplayManager> {

		#region Constant Variables

		#endregion

		#region Static Variables
		public delegate void OnGameStart();
		public event OnGameStart ON_GAME_START;

		public delegate void OnTileSelect();
		public event OnTileSelect ON_TILE_SELECT;

		public delegate void OnTilePlaced();
		public event OnTilePlaced ON_TILE_PLACED;

		public delegate void OnEndTurn();
		public event OnEndTurn ON_END_TURN;

		public delegate void OnGameEnd();
		public event OnGameEnd ON_GAME_END;
		#endregion

		#region Public Variables
		public GameObject PlacementTile;
		#endregion

		#region Private Variables
		[Header("Settings")]
		[SerializeField] private float tilePlaceSpeed = 1.5f;
		[SerializeField] [Range(1, 3)] private int minAIStackAmount = 1; //When the ai will start considering stacking
		[SerializeField] private bool allowViableTileRandomAIChoice; //Meant to add additional variable to v/h/d viable stack options

		[Header("Runtime Debug")]
		public string PlayerName;
		public Color PlayerTeamColor;
		public string BotName;
		public Color AITeamColor;
		public bool IsPlayerTurn;
		public int ActionsTaken;
		public int CurrentRound;
		public GridTile CurrentDestinationTile;
		public bool WinStateFound;
		public WinDirectionType WinDirectionType;

		private Coroutine tileDropProcess;
		private Coroutine aiThinkProcess;
		#endregion

		#region Unity Methods
		private void Awake() {
			PlacementTile.SetActive(false);
		}

		private void Update() {
			Application.targetFrameRate = 30;
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void StartGame() {
			PlacementTile.SetActive(false);
			if (aiThinkProcess != null) {
				StopCoroutine(aiThinkProcess);
				aiThinkProcess = null;
			}
			if (tileDropProcess != null) {
				StopCoroutine(tileDropProcess);
				tileDropProcess = null;
			}

			GridManager.Instance.PrepareGrid();
			WinStateFound = false;
			WinDirectionType = WinDirectionType.None;
			ActionsTaken = 0;
			CurrentRound = 1;

			if (UnityEngine.Random.Range(0, 2) == 0) {
				IsPlayerTurn = true;
			} else {
				IsPlayerTurn = false;
			}
			ON_GAME_START?.Invoke();
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
			CurrentDestinationTile = gridTile;
			StartTileDropProcess();
			GridManager.Instance.HideTiles();
			ON_TILE_SELECT?.Invoke();
		}

		public void EndPlayerTurn() { //Removed the manual player linked button functionality instead for more decisive gameplay
			GridManager.Instance.HideTiles();
			EndGlobalTurn();
			if (CurrentRound == 22) {
				SetWinState();
				return;
			}
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
			EndGlobalTurn();
			if (CurrentRound == 22) {
				SetWinState();
				return;
			}
			StartPlayerTurn();
		}
		#endregion

		#region Local Methods
		private void StartTileDropProcess() {
			if (tileDropProcess != null) {
				StopCoroutine(tileDropProcess);
				tileDropProcess = null;
			}
			tileDropProcess = StartCoroutine(TileDropProcess());
		}

		private IEnumerator TileDropProcess() { //Make this an animation and just wait animation time (add bounce effect to anim)
			bool isTileDropping = true;
			do {
				PlacementTile.transform.GetChild(0).GetComponent<Image>().color = IsPlayerTurn ? PlayerTeamColor : AITeamColor;
				PlacementTile.SetActive(true);
				if (isTileDropping) {
					PlacementTile.transform.position = Vector3.MoveTowards(PlacementTile.transform.position, CurrentDestinationTile.transform.position, tilePlaceSpeed);
					//PlacementTile.transform.position = StaticUtilities.MoveTowards(PlacementTile.transform.position, DestinationTile.transform.position, tilePlaceSpeed);
					if (StaticUtilities.DistanceThreshold(PlacementTile.transform.position, CurrentDestinationTile.transform.position, 0.1f)) {
						CurrentDestinationTile.PlaceTile();
						AnalyzeBoardState();
						isTileDropping = false;
						CurrentDestinationTile = null;
						ON_TILE_PLACED?.Invoke();
					}
				}
				yield return new WaitForSeconds(0.01f);
			} while (isTileDropping);
			PlacementTile.SetActive(false);
		}

		private IEnumerator AIThinkProcess() {
			yield return new WaitForSeconds(.5f); //Allow the player a moment to let the ai "think"
			AISelectBestTile();

		}

		private void AISelectBestTile() {
			GridTile verticalTile = null;
			GridTile horizontalTile = null;
			GridTile diagonalTile = null;
			GridTile selectedTile = null;

			verticalTile = AIVerticalStackCheck();
			horizontalTile = AIHorizontalStackCheck();

			int viableCount = 0;
			if (verticalTile != null) {
				viableCount++;
				if (horizontalTile != null) {
					viableCount++;
				}
				if (diagonalTile != null) {
					viableCount++;
				}
				if (viableCount == 1) {
					Debug.Log("AI chose verticalTile");
					selectedTile = verticalTile;
				} else {
					Debug.Log($"AI viableCount = {viableCount}");
					int randTile = 0;
					if (allowViableTileRandomAIChoice) {
						randTile = UnityEngine.Random.Range(0, viableCount + 1);
					} else {
						randTile = UnityEngine.Random.Range(0, viableCount);
					}
					Debug.Log($"AI randTile = {randTile}");
					switch (randTile) {
						case 0:
							Debug.Log("AI chose verticalTile");
							selectedTile = verticalTile;
							break;
						case 1:
							if (horizontalTile != null) {
								Debug.Log("AI chose horizontalTile");
								selectedTile = horizontalTile;
							}
							if (diagonalTile != null) {
								Debug.Log("AI chose diagonalTile");
								selectedTile = diagonalTile;
							}
							break;
						case 2:
							Debug.Log("AI chose diagonalTile");
							selectedTile = diagonalTile;
							break;
						case 3:
							selectedTile = null;
							break;
					}
				}
			}

			if (selectedTile == null) { //Cant find a good tile placement
				GridTile[] viableGridTiles = GridManager.Instance.GetViableGridTiles();
				if (viableGridTiles != null && viableGridTiles.Length > 0) {
					Debug.Log("AI chose random");
					selectedTile = viableGridTiles[UnityEngine.Random.Range(0, viableGridTiles.Length)];
					if (selectedTile != null) {
						selectedTile.SelectTile();
					}
				}
			} else {
				selectedTile.SelectTile();
			}
		}

		private GridTile AIVerticalStackCheck() {
			GridTile foundTile = null;
			int verticalMatches = 0;
			for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
				verticalMatches = 0;
				for (int tile = GridManager.Instance.gridLanes[lane].gridTiles.Length; tile-- > 0;) { //Bottom->Top
					if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated) {
						if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
							verticalMatches++;
							if (verticalMatches >= minAIStackAmount) {
								if (tile > 0) { //Make sure that we are not checking a non existing tile
									if (!GridManager.Instance.gridLanes[lane].gridTiles[tile - 1].IsPopulated) { //Need to make sure the tile above the current tile is not populated
										foundTile = GridManager.Instance.gridLanes[lane].gridTiles[tile - 1];
										break;
									}
								}
							}
						} else {
							verticalMatches = 0;
						}
					} else {
						verticalMatches = 0;
					}
				}
				if (foundTile) {
					Debug.Log($"Found vertical tile {foundTile.gameObject.name}/{foundTile.AssignedLane}");
					break;
				}
			}
			return foundTile;
		}

		private GridTile AIHorizontalStackCheck() {
			GridTile foundTile = null;
			int horizontalMatches = 0;
			for (int tile = 6; tile-- > 0;) { //Bottom->Top
				horizontalMatches = 0;
				for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
					if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated) {
						if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
							horizontalMatches++;
							if (horizontalMatches >= minAIStackAmount) {
								if (lane > 0) { //Make sure that we are not checking a non existing left lane
									if (!GridManager.Instance.gridLanes[lane - 1].gridTiles[tile].IsPopulated && //Need to check that the tile on the lane to the left is not populated
										((tile + 1 <= 5) && GridManager.Instance.gridLanes[lane - 1].gridTiles[tile + 1].IsPopulated)) { //Need to check if the tile on the lane to the left and down 1 exists and is populated
										foundTile = GridManager.Instance.gridLanes[lane - 1].gridTiles[tile];
										break;
									}
								}
								if (lane < 5) { //Make sure that we are not checking a non existing right lane
									if (!GridManager.Instance.gridLanes[lane + 1].gridTiles[tile].IsPopulated && //Need to check that the tile on the lane to the right is not populated
										((tile + 1 <= 5) && GridManager.Instance.gridLanes[lane + 1].gridTiles[tile + 1].IsPopulated)) { //Need to check if the tile on the lane to the right and down 1 exists and is populated
										foundTile = GridManager.Instance.gridLanes[lane + 1].gridTiles[tile];
										break;
									}
								}
							}
						} else {
							horizontalMatches = 0;
						}
					} else {
						horizontalMatches = 0;
					}
				}
				if (foundTile) {
					Debug.Log($"Found horizontal tile {foundTile.gameObject.name}/{foundTile.AssignedLane}");
					break;
				}
			}
			return foundTile;
		}

		private void EndGlobalTurn() {
			IsPlayerTurn = !IsPlayerTurn;
			ActionsTaken++;
			if (ActionsTaken == 2) {
				CurrentRound++;
				ActionsTaken = 0;
			}
			ON_END_TURN?.Invoke();
		}

		private void AnalyzeBoardState() {
			//Vertical
			if (!WinStateFound) {
				WinStateFound = VerticalWinCheck();
			}

			//Horizontal
			if (!WinStateFound) {
				WinStateFound = HorizontalWinCheck();
			}

			//Diagonal
			if (!WinStateFound) {
				WinStateFound = DiagonalWinCheck();
			}

			CheckWinState();
		}

		private bool VerticalWinCheck() {
			bool foundWinScenario = false;
			int verticalMatches = 0;
			for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
				verticalMatches = 0;
				for (int tile = GridManager.Instance.gridLanes[lane].gridTiles.Length; tile-- > 0;) { //Bottom->Top
					if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated) {
						if (IsPlayerTurn) {
							if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
								verticalMatches++;
								if (verticalMatches == 4) {
									foundWinScenario = true;
									break;
								}
							} else {
								verticalMatches = 0;
							}
						} else {
							if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
								verticalMatches++;
								if (verticalMatches == 4) {
									foundWinScenario = true;
									break;
								}
							} else {
								verticalMatches = 0;
							}
						}
					} else {
						verticalMatches = 0;
					}
				}
				if (foundWinScenario) {
					Debug.Log("Vertical win");
					WinDirectionType = WinDirectionType.Vertical;
					break;
				}
			}
			return foundWinScenario;
		}

		private bool HorizontalWinCheck() {
			bool foundWinScenario = false;
			int horizontalMatches = 0;
			for (int tile = 6; tile-- > 0;) { //Bottom->Top
				horizontalMatches = 0;
				for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
					if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated) {
						if (IsPlayerTurn) {
							if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
								horizontalMatches++;
								if (horizontalMatches == 4) {
									foundWinScenario = true;
									break;
								}
							} else {
								horizontalMatches = 0;
							}
						} else {
							if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned) {
								horizontalMatches++;
								if (horizontalMatches == 4) {
									foundWinScenario = true;
									break;
								}
							} else {
								horizontalMatches = 0;
							}
						}
					} else {
						horizontalMatches = 0;
					}
				}
				if (foundWinScenario) {
					Debug.Log("Horizontal win");
					WinDirectionType = WinDirectionType.Horizontal;
					break;
				}
			}
			return foundWinScenario;
		}

		private bool DiagonalWinCheck() {
			bool foundWinScenario = false;
			for (int tile = 6; tile-- > 0;) { //Bottom->Top
				for (int lane = GridManager.Instance.gridLanes.Length; lane-- > 0;) { //Right->Left
					if (tile - 4 >= 0 && lane - 4 >= 0) {
						if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated &&
						GridManager.Instance.gridLanes[lane - 1].gridTiles[tile - 1].IsPopulated &&
						GridManager.Instance.gridLanes[lane - 2].gridTiles[tile - 2].IsPopulated &&
						GridManager.Instance.gridLanes[lane - 3].gridTiles[tile - 3].IsPopulated) {
							if (IsPlayerTurn) {
								if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane - 1].gridTiles[tile - 1].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane - 2].gridTiles[tile - 2].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane - 3].gridTiles[tile - 3].IsPlayerOwned) {
									foundWinScenario = true;
									break;
								}
							} else {
								if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane - 1].gridTiles[tile - 1].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane - 2].gridTiles[tile - 2].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane - 3].gridTiles[tile - 3].IsPlayerOwned) {
									foundWinScenario = true;
									break;
								}
							}
						}
					}
				}
				if (foundWinScenario) {
					break;
				}
			}
			for (int tile = 6; tile-- > 0;) { //Bottom->Top
				for (int lane = 0; lane < GridManager.Instance.gridLanes.Length; lane++) { //Left->Right
					if (tile - 4 >= 0 && lane + 4 <= GridManager.Instance.gridLanes.Length) {
						if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPopulated &&
						GridManager.Instance.gridLanes[lane + 1].gridTiles[tile - 1].IsPopulated &&
						GridManager.Instance.gridLanes[lane + 2].gridTiles[tile - 2].IsPopulated &&
						GridManager.Instance.gridLanes[lane + 3].gridTiles[tile - 3].IsPopulated) {
							if (IsPlayerTurn) {
								if (GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane + 1].gridTiles[tile - 1].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane + 2].gridTiles[tile - 2].IsPlayerOwned &&
									GridManager.Instance.gridLanes[lane + 3].gridTiles[tile - 3].IsPlayerOwned) {
									foundWinScenario = true;
									break;
								}
							} else {
								if (!GridManager.Instance.gridLanes[lane].gridTiles[tile].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane + 1].gridTiles[tile - 1].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane + 2].gridTiles[tile - 2].IsPlayerOwned &&
									!GridManager.Instance.gridLanes[lane + 3].gridTiles[tile - 3].IsPlayerOwned) {
									foundWinScenario = true;
									break;
								}
							}
						}
					}
				}
				if (foundWinScenario) {
					Debug.Log("Diagonal win");
					WinDirectionType = WinDirectionType.Diagonal;
					break;
				}
			}
			return foundWinScenario;
		}

		private void CheckWinState() {
			if (WinStateFound) {
				SetWinState();
			} else {
				if (CurrentDestinationTile.IsPlayerOwned) {
					EndPlayerTurn();
				} else {
					EndAITurn();
				}
			}
		}

		private void SetWinState() {
			Debug.Log("Winner winner");
			ON_GAME_END?.Invoke();
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums
	[Serializable]
	public enum WinDirectionType { None, Vertical, Horizontal, Diagonal };
	#endregion
}