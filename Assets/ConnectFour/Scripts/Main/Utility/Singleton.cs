using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLucas {

	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		static T _instance;
		public static T Instance {
			get {
				if (_instance == null) {
					T obj = (T)FindObjectOfType(typeof(T));
					if (obj == null) {
						Debug.LogWarning("Cannot find singleton of type '" + typeof(T).Name + "'");
					} else {
						_instance = obj;
					}
					return _instance;
				} else {
					return _instance;
				}
			}
		}
	}
}
