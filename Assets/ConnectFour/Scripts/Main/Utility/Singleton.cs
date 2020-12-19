using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLucas {

	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		static T _instance;
		public static T Instance {
			get {
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));
					if (_instance == null) {
						var obj = new GameObject();
						obj.name = typeof(T).ToString();
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;
			}
		}
		public virtual void Awake() {
			if (_instance != this) {
				Destroy(gameObject);
				return;
			}
			_instance = GetComponent<T>();
		}
	}

	public abstract class SingletonDND<T> : MonoBehaviour where T : MonoBehaviour {

		static T _instance;
		public static T Instance {
			get {
				Debug.Log("here");
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));
					if (_instance == null) {
						var obj = new GameObject();
						obj.name = typeof(T).ToString();
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;
			}
		}
		public virtual void Awake() {
			if (_instance != this) {
				Debug.Log("killed");
				Destroy(gameObject);
				return;
			}
			_instance = GetComponent<T>();
			DontDestroyOnLoad(gameObject);
		}
	}
}
