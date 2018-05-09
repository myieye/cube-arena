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
			SetColour(hoverColour);
		}

		public void Select() {
			SetColour(selectColour);
		}

		public void Unhighlight() {
			SetColour(baseColor);
		}

		public void MarkDisallowed() {
			SetColour(disallowedColor);
		}
	}
}