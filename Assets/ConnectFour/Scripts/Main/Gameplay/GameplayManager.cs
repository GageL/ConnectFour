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
		[SerializeField] private float tilePlaceSpeed = 1.5f;

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
					PlacementTile.transform.position = Vector3.MoveTowards(PlacementTile.transform.position, CurrentDestinationTile.transform.position, tilePlaceSpeed * Time.deltaTime);
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
			yield return new WaitForEndOfFrame(); //Make the bot move instantly (use waitseconds for think behavior)
			AISelectBestTile();

		}

		private void AISelectBestTile() {
			GridTile selectedTile = null;

			selectedTile = AIVerticalStackCheck();

			//Cant find a good tile placement
			if (selectedTile == null) {
				GridTile[] viableGridTiles = GridManager.Instance.GetViableGridTiles();
				if (viableGridTiles != null && viableGridTiles.Length > 0) {
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
							if (verticalMatches >= 1) {
								if (tile > 0) {
									if (!GridManager.Instance.gridLanes[lane].gridTiles[tile - 1].IsPopulated) {
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
					Debug.Log($"Found tile {foundTile.gameObject.name}/{foundTile.AssignedLane}");
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