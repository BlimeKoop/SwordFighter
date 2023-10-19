using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonViewChildDestroyer : MonoBehaviour
{
    [HideInInspector] public PhotonView PhotonView;

    private void Awake()
    {
        PhotonView = GetComponentInParent<PhotonView>();
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
