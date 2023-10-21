using UnityEngine;
using Photon.Pun;

public class PhotonViewRenamer : MonoBehaviour, IPunInstantiateMagicCallback
{
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null)
        {
            return;
        }

        if (info.photonView.InstantiationData[0] != null)
        {
            gameObject.name = (string) info.photonView.InstantiationData[0];
        }
    }
}
