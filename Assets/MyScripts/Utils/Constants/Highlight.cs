using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
	public static class Highlight {
		public const float HoverHighlight = 0.2f;
		public const float SelectHighlight = 0.4f;
		public const float DisallowHighlight = 0.4f;
		public const float CursorTransparency = 0.2f;

		public static Color GetHighlightedColor (Color color, float highlight) {
			return new Color (color.r + highlight, color.g + highlight, color.b + highlight);
		}

		public static Color ReduceTransparency (Color color, float highlight) {
			color.a -= highlight;
			return color;
		}
	}
}