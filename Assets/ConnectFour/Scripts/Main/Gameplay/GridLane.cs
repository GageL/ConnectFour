using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace C4 {
	public class GridLane : MonoBehaviour {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables
		[Header("Runtime Debug")]
		public int LaneIndex;
		public GridTile[] gridTiles;
		#endregion

		#region Unity Methods
		private void Awake() {
			gridTiles = this.transform.GetComponentsInChildren<GridTile>(false);
			for (int i = 0; i < gridTiles.Length; i++) {
				gridTiles[i].AssignedLane = this.transform.GetSiblingIndex();
			}
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void ShowPlaceableTile() {
			for (int i = gridTiles.Length; i-- > 0;) {
				if (!gridTiles[i].IsPopulated) {
					gridTiles[i].ShowPlaceable();
					break;
				}
			}
		}

		public void HidePlaceableTile() {
			for (int i = gridTiles.Length; i-- > 0;) {
				if (!gridTiles[i].IsPopulated) {
					gridTiles[i].HidePlaceable();
				}
			}
		}

		public void ResetTiles() {
			for (int i = 0; i < gridTiles.Length; i++) {
				gridTiles[i].ResetTile();
			}
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