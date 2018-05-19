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
        private PlayerId playerId;
        private NetworkConnection authoratativeConnection;
        private NetworkIdentity[] cubeNetworkIds;

        private void Start () {
            if (isServer) {
                playerId = GetComponent<PlayerId> ();
                authoratativeConnection = PlayerManager.Instance.GetPlayerConnection (playerId);// GetComponent<NetworkIdentity> ().clientAuthorityOwner;
                cubeNetworkIds = PlayerManager.Instance.GetPlayerCubes (playerId)
                    .Select (cube => cube.GetComponent<NetworkIdentity> ()).ToArray ();
            } else {
                
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
        private void CmdSetCubeLocalAuthority (bool takeLocalAuthority) {
            if (takeLocalAuthority == cubesHaveAuthority) return;

            foreach (var cubeNetworkId in cubeNetworkIds) {
                if (takeLocalAuthority) {
                    cubeNetworkId.localPlayerAuthority = true;
                    var success = cubeNetworkId.AssignClientAuthority (authoratativeConnection);
                    Debug.LogFormat ("Giving Authority of {0} to {1} [{2}]", cubeNetworkId.gameObject.name, authoratativeConnection.connectionId, success);
                } else {
                    var success = cubeNetworkId.RemoveClientAuthority (authoratativeConnection);
                    cubeNetworkId.localPlayerAuthority = false;
                    Debug.LogFormat ("Remvoing Authority of {0} from {1} [{2}]", cubeNetworkId.gameObject.name, authoratativeConnection.connectionId, success);
                }
            }

            cubesHaveAuthority = takeLocalAuthority;
        }
    }
}