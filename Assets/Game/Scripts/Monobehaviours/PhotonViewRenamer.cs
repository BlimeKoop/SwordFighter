using UnityEngine;
using Photon.Pun;

public class PhotonViewRenamer : MonoBehaviour, IPunInstantiateMagicCallback
{
    private PhotonView PhotonView;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PhotonView = GetComponent<PhotonView>();

        if (info.photonView.InstantiationData != null)
            gameObject.name = (string) info.photonView.InstantiationData[0];
        else
            gameObject.name += $" {PhotonView.ViewID}";

        if (transform.childCount > 0)
            foreach (PhotonViewRenamer p in transform.GetChild(0).GetComponentsInChildren<PhotonViewRenamer>())
                p.OnPhotonInstantiate(info);
    }
}
