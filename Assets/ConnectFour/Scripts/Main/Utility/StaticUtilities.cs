using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C4.Utilities {
	public class StaticUtilities {

		#region Public Variables

		#endregion

		#region Private Variables

		#endregion

		#region Local Variables

		#endregion

		#region Main Functions

		#endregion

		#region Utility Functions

		#endregion

		#region Public Functions
		public static bool IsStringEmpty(string value) {
			return (value == string.Empty ? true : false);
		}

		public static bool HasWhitespace(string value) {
			return (value == string.Empty ? false : value.Contains(" "));
		}

		public static bool HasSpecialChar(string value) {
			return (IsStringEmpty(value) ? false : !Regex.IsMatch(value, "^[a-zA-Z0-9]*$"));
		}

		public static Vector3 MoveTowards(Vector3 currentValue, Vector3 targetValue, float lerpRate) {
			currentValue = Vector3.MoveTowards(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static Vector3 Vector3Lerp(Vector3 currentValue, Vector3 targetValue, float lerpRate) {
			currentValue = Vector3.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static Vector3 Vector2Lerp(Vector2 currentValue, Vector2 targetValue, float lerpRate) {
			currentValue = Vector2.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static float FloatLerp(float currentValue, float targetValue, float lerpRate) {
			currentValue = Mathf.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static bool DistanceThreshold(Vector3 pos1, Vector3 pos2, float dist) { //More efficient than Vector3.Distance()
			if ((pos1 - pos2).sqrMagnitude <= dist * dist) {
				return true;
			} else {
				return false;
			}
		}

		public static string StyleText(string input, Color color, bool isBold) {
			string format = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), input);
			if (isBold) {
				format = ("<b>" + format + "</b>");
			}
			return format;
		}

		public static string StyleText(string input, string hexColor, bool isBold) {
			string format = string.Format("<color=#{0}>{1}</color>", hexColor, input);
			if (isBold) {
				format = ("<b>" + format + "</b>");
			}
			return format;
		}

		public static string StyleText(string input, Color color, int size, bool isBold) {
			string format = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), input);
			format = ("<size=" + size + ">" + format + "</size>");
			if (isBold) {
				format = ("<b>" + format + "</b>");
			}
			return format;
		}

		public static string StyleText(string input, string hexColor, int size, bool isBold) {
			string format = string.Format("<color=#{0}>{1}</color>", hexColor, input);
			format = ("<size=" + size + ">" + format + "</size>");
			if (isBold) {
				format = ("<b>" + format + "</b>");
			}
			return format;
		}

		public static string SpaceUppercase(string input) {
			return Regex.Replace(input, "[A-Z]", " $0");
		}

		public static string FromCamelCase(string input) {
			string format = char.ToUpper(input[0]) + input.Substring(1);
			return Regex.Replace(Regex.Replace(format, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
		}

		public static float FloatTruncate(float value, int digits) {
			double mult = System.Math.Pow(10.0, digits);
			double result = System.Math.Truncate(mult * value) / mult;
			return (float)result;
		}

		public static string GenerateGUID(string prefix) {
			string value = string.Empty;
			if (!string.IsNullOrEmpty(prefix)) {
				value = prefix + "|";
			}
			return value + System.Guid.NewGuid().ToString();
		}

		public static bool IsEvenNumber(int value) {
			bool result = false;
			if (value % 2 == 0) {
				result = true;
			} else {
				result = false;
			}
			return result;
		}

		public static T EnumFromString<T>(string value) {
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static bool ArrayContains<T>(IEnumerable array, object value) {
			foreach (var v in array) {
				if (v == value) {
					return true;
				}
			}
			return false;
		}

		public static bool IsHeadlessServer() {
			return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
		}

		public static bool IsNullOrEmpty(string input) {
			return (string.IsNullOrEmpty(input) || (input == "null"));
		}
		#endregion
	}

	#region Base Classes

	#endregion

	#region Base Enums

	#endregion
}