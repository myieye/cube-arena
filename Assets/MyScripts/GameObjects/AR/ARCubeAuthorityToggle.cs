using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {

    [RequireComponent (typeof (PlayerId), typeof (NetworkIdentity))]
    public class ARCubeAuthorityToggle : NetworkBehaviour {

        [SyncVar]
        private bool cubesHaveAuthority;
        private NetworkConnection authoratativeConnection;
        private IEnumerable<NetworkIdentity> cubeNetworkIds;

        private void Start () {
            if (!isServer) return;

            authoratativeConnection = GetComponent<NetworkIdentity> ().clientAuthorityOwner;
            cubeNetworkIds = PlayerManager.Instance.GetPlayerCubes (GetComponent<PlayerId> ())
                .Select (cube => cube.GetComponent<NetworkIdentity> ());
            foreach (var netId in cubeNetworkIds) {
                netId.localPlayerAuthority = false;
            }
        }

        private void Update () {
            if (!hasAuthority) return;

            if (!cubesHaveAuthority && ARManager.WorldEnabled) {
                CmdSetCubeLocalAuthority (true);
            } else if (cubesHaveAuthority && !ARManager.WorldEnabled) {
                CmdSetCubeLocalAuthority (false);
            }
        }

        [Command]
        private void CmdSetCubeLocalAuthority (bool takeAuthority) {
            if (takeAuthority == cubesHaveAuthority) return;

            foreach (var cubeNetworkId in cubeNetworkIds) {
                if (takeAuthority) {
                    cubeNetworkId.localPlayerAuthority = true;
                    cubeNetworkId.AssignClientAuthority (authoratativeConnection);
                } else {
                    cubeNetworkId.RemoveClientAuthority (authoratativeConnection);
                    cubeNetworkId.localPlayerAuthority = false;
                }
            }

            cubesHaveAuthority = takeAuthority;
        }
    }
}