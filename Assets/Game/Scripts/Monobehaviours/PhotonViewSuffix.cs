using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonViewSuffix : PhotonViewRenamer
{
    private PhotonView PhotonView;
	
	private bool suffixApplied;

	private void Start()
	{
		if (!suffixApplied)
		{
			PhotonView = GetComponentInParent<PhotonView>();

			ApplySuffix($"{PhotonView.ViewID}");

			if (transform.childCount > 0)
				foreach (PhotonViewSuffix p in transform.GetChild(0).GetComponentsInChildren<PhotonViewSuffix>())
					p.ApplySuffix($"{PhotonView.ViewID}");
		}
	}

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
		base.OnPhotonInstantiate(info);
		
		if (suffixApplied)
			return;
	
        PhotonView = GetComponentInParent<PhotonView>();

        ApplySuffix($"{PhotonView.ViewID}");

        if (transform.childCount > 0)
            foreach (PhotonViewSuffix p in transform.GetChild(0).GetComponentsInChildren<PhotonViewSuffix>())
                p.ApplySuffix($"{PhotonView.ViewID}");
    }

    public void ApplySuffix(string suffix)
    {
        gameObject.name += $" {suffix}";
		
		suffixApplied = true;
    }
}
