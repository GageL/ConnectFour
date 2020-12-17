using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLucas;

namespace C4 {
	public class GameplayUI : Singleton<GameplayUI> {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables

		#endregion

		#region Unity Methods
		private void Awake() {
			GameplayUI_PregameMenu.Instance.RootHolder.SetActive(true);
			GameplayUI_IngameMenu.Instance.RootHolder.SetActive(false);
			GameplayUI_PostgameMenu.Instance.RootHolder.SetActive(false);
		}
		#endregion

		#region Callback Methods

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