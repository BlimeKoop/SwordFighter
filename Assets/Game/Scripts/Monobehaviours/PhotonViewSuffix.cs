using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonViewSuffix : PhotonViewRenamer
{
    private PhotonView PhotonView;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
		base.OnPhotonInstantiate(info);
		
        PhotonView = GetComponent<PhotonView>();

        ApplySuffix($"{PhotonView.ViewID}");

        if (transform.childCount > 0)
            foreach (PhotonViewSuffix p in transform.GetChild(0).GetComponentsInChildren<PhotonViewSuffix>())
                p.ApplySuffix($"{PhotonView.ViewID}");
    }

    public void ApplySuffix(string suffix)
    {
        gameObject.name += $" {suffix}";
    }
}
