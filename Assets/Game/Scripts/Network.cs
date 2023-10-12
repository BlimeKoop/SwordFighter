using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static UnityEngine.GraphicsBuffer;

public static class Network
{
    public static void InstantiateCutParents(GameObject cutObj)
    {
        string objName = cutObj.name;

        GameObject first = PhotonNetwork.Instantiate(
            "Rigidbody", Vector3.zero, Quaternion.identity, 0, new object[] { objName + $" (1/2)" });
        GameObject second = PhotonNetwork.Instantiate(
            "Rigidbody", Vector3.zero, Quaternion.identity, 0, new object[] { objName + $" (2/2)" });
    }
}