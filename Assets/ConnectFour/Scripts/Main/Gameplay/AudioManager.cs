using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLucas;

namespace C4 {
	public class AudioManager : Singleton<AudioManager> {

		#region Constant Variables

		#endregion

		#region Static Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables
		[SerializeField] AudioSource musicAudioSource;
		[SerializeField] AudioClip lobbyTrack;
		[SerializeField] AudioClip gameplayTrack;
		[SerializeField] AudioSource effectsAudioSource;
		[SerializeField] AudioClip pieceSelectClip;
		[SerializeField] AudioClip pieceMoveClip;
		[SerializeField] AudioClip piecePlaceClip;
		[SerializeField] AudioClip winStateClip;
		[SerializeField] AudioClip loseStateClip;

		private Coroutine fadeTrackProcess;
		#endregion

		#region Unity Methods
		public override void Awake() {
			base.Awake();
			musicAudioSource.volume = 0;
			StartCoroutine(FadeTrackProcess(false));
		}

		private void OnEnable() {
			GameplayManager.ON_GAME_START += OnGameStart;
			GameplayManager.ON_TILE_SELECT += OnTileSelect;
			GameplayManager.ON_TILE_PLACED += OnTilePlaced;
			GameplayManager.ON_GAME_END += OnGameEnd;
		}

		private void OnDisable() {
			GameplayManager.ON_GAME_START -= OnGameStart;
			GameplayManager.ON_TILE_SELECT -= OnTileSelect;
			GameplayManager.ON_TILE_PLACED -= OnTilePlaced;
			GameplayManager.ON_GAME_END -= OnGameEnd;
		}
		#endregion

		#region Callback Methods
		private void OnGameStart() {
			if (fadeTrackProcess != null) {
				StopCoroutine(fadeTrackProcess);
				fadeTrackProcess = null;
			}
			fadeTrackProcess = StartCoroutine(FadeTrackProcess(false));
			PlayGameplayTrack();
		}

		private void OnTileSelect() {
			if (GameplayManager.Instance.IsPlayerTurn) {
				PlayOneShot(pieceSelectClip);
			}
		}

		private void OnTilePlaced() {
			PlayOneShot(piecePlaceClip);
		}

		private void OnGameEnd() {
			if (fadeTrackProcess != null) {
				StopCoroutine(fadeTrackProcess);
				fadeTrackProcess = null;
			}
			fadeTrackProcess = StartCoroutine(FadeTrackProcess(true));
			if (GameplayManager.Instance.IsPlayerTurn) {
				PlayOneShot(winStateClip);
			} else {
				PlayOneShot(loseStateClip);
			}
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void PlayLobbyTrack() {
			musicAudioSource.Stop();
			musicAudioSource.clip = lobbyTrack;
			musicAudioSource.Play();
		}

		public void PlayButtonClick() {
			PlayOneShot(pieceSelectClip);
		}
		#endregion

		#region Local Methods
		private void PlayGameplayTrack() {
			musicAudioSource.Stop();
			musicAudioSource.clip = gameplayTrack;
			musicAudioSource.Play();
		}

		private void PlayOneShot(AudioClip clip) {
			effectsAudioSource.PlayOneShot(clip);
		}

		private IEnumerator FadeTrackProcess(bool descending) {
			while (descending ? musicAudioSource.volume > 0 : musicAudioSource.volume < 0.1f) {
				if (descending) {
					musicAudioSource.volume -= 0.1f * Time.deltaTime;
				} else {
					musicAudioSource.volume += 0.1f * Time.deltaTime;
				}
				yield return new WaitForEndOfFrame();
			}
		}
		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}