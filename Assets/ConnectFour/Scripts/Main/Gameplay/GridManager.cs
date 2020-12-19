using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLucas;

namespace C4 {
	public class GridManager : Singleton<GridManager> {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables
		public Transform[] LanePlacements;
		#endregion

		#region Private Variables
		[Header("Runtime Debug")]
		public GridLane[] gridLanes;
		#endregion

		#region Unity Methods
		public void Awake() {
			gridLanes = this.GetComponentsInChildren<GridLane>(false);
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		#endregion

		#region Public Methods
		public void PrepareGrid() {
			ResetLanes();
		}

		public void ShowTiles() {
			for (int i = 0; i < gridLanes.Length; i++) {
				gridLanes[i].ShowPlaceableTile();
			}
		}

		public void HideTiles() {
			for (int i = 0; i < gridLanes.Length; i++) {
				gridLanes[i].HidePlaceableTile();
			}
		}

		public GridTile[] GetViableGridTiles() {
			List<GridTile> viableTiles = new List<GridTile>();
			for (int a = 0; a < gridLanes.Length; a++) {
				for (int b = gridLanes[a].gridTiles.Length; b-- > 0;) {
					if (!gridLanes[a].gridTiles[b].IsPopulated) {
						viableTiles.Add(gridLanes[a].gridTiles[b]);
						break;
					}
				}
			}
			return viableTiles.ToArray();
		}

		public GridTile[] GetAIGridTiles() {
			List<GridTile> aiTiles = new List<GridTile>();
			for (int a = 0; a < gridLanes.Length; a++) {
				for (int b = gridLanes[a].gridTiles.Length; b-- > 0;) {
					if (gridLanes[a].gridTiles[b].IsPopulated && !gridLanes[a].gridTiles[b].IsPlayerOwned) {
						aiTiles.Add(gridLanes[a].gridTiles[b]);
						break;
					}
				}
			}
			return aiTiles.ToArray();
		}
		#endregion

		#region Local Methods
		private void ResetLanes() {
			for (int i = 0; i < gridLanes.Length; i++) {
				gridLanes[i].ResetTiles();
			}
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}