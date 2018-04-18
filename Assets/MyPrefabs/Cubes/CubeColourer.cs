using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;

namespace CubeArena.Assets.MyPrefabs.Cubes
{
	public class CubeColourer : Colourer {

		private Color baseColor;
		private Color hoverColour;
		private Color selectColour;
		private Color disallowedColor;
		
		public override void Start() {
			base.Start();
			baseColor = color;
			hoverColour = Highlight.GetHighlightedColor(baseColor, Highlight.HoverHighlight);
			selectColour = Highlight.GetHighlightedColor(baseColor, Highlight.SelectHighlight);
			disallowedColor = Highlight.ReduceTransparency(baseColor, Highlight.DisallowHighlight);
		}

		public void Hover() {
			SetColor(hoverColour);
		}

		public void Select() {
			SetColor(selectColour);
		}

		public void Unhighlight() {
			SetColor(baseColor);
		}

		public void MarkDisallowed() {
			SetColor(disallowedColor);
		}
	}
}