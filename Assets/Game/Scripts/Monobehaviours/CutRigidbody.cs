using UnityEngine;
using Photon.Pun;

public class CutRigidbody : MonoBehaviour, IPunInstantiateMagicCallback
{
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
		var instantiationData = info.photonView.InstantiationData;
		
		if (instantiationData != null)
		{
			gameObject.tag = (string) instantiationData[1];
			transform.localScale = (Vector3) instantiationData[2];
		}
    }
}
