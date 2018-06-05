using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueShower : MonoBehaviour {

	[SerializeField]
	private Text text;
	[SerializeField]
	private Slider slider;
	
	public void UpdateValue () {
		text.text = string.Format ("({0})", slider.value);
	}
}
