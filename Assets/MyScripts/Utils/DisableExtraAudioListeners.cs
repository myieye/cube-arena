﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class DisableExtraAudioListeners : NetworkBehaviour {

		void Start () {
			if(!hasAuthority) {
				GetComponent<AudioListener>().enabled = false;
			}
		}
	}
}