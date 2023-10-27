using UnityEngine;
using Photon.Pun;

public class PhotonViewRenamer : MonoBehaviour, IPunInstantiateMagicCallback
{
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null || info.photonView.InstantiationData[0] == null)
            return;

		gameObject.name = (string) info.photonView.InstantiationData[0];
	}
}
