using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Drawing;
using Unity.VisualScripting;

namespace DynamicMeshCutter
{
	public class SwordCutterBehaviour : CutterBehaviour
	{
		private PhotonView photonView;

        bool initialized, cutting;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();

            initialized = true;
        }

        [PunRPC]
		public void CutObject(string objectName, Vector3 cutAxis, Vector3 point)
        {
            if (!initialized)
                Start();

            Debug.Log("Going to cut " + objectName);
			
            StartCoroutine(CutObjectWhenReady(objectName, cutAxis, point));
        }

		private IEnumerator CutObjectWhenReady(string objectName, Vector3 cutAxis, Vector3 point)
		{
			// This client may still be cutting something so wait if that's the case
			yield return new WaitWhile(() => cutting);
			
			cutting = true;
			
			GameObject obj = GameObject.Find(objectName);
			
			MeshTarget meshTarget = InitializeMeshTarget(obj);
			
			Debug.Log(meshTarget == null);
			
            // If i'm not the client instantiating the rigidbodies on the server
            if (!photonView.IsMine)	
            {
                // Should probably wait until they're spawned
                yield return new WaitUntil(() => FoundObject($"{meshTarget.gameObject.name} {RoomManager.sliceCount} (1/"));
            }

			PlayerController playerController = meshTarget.GetComponentInParent<PlayerController>();
			
			if (playerController != null)
				playerController.Die();

			Debug.Log("Cutting " + objectName);

            Cut(meshTarget, point, cutAxis.normalized, null, OnCreated, null,
			meshTarget.GetComponentInParent<PhotonView>(), photonView.IsMine);
        }

        private bool FoundObject(string searchFor)
        {
            foreach(PhotonView view in FindObjectsOfType<PhotonView>())
            {
                if (view.gameObject.name.Contains(searchFor))
                {
                    return true;
                }
            }

            return false;
        }

		private MeshTarget InitializeMeshTarget(GameObject obj)
		{
			MeshTarget meshTargetR = Objects.GetComponentInFamily<MeshTarget>(obj);
			
			if (meshTargetR == null)
			{
				if (Objects.GetComponentInFamily<Renderer>(obj) == null)
					return null;
				
				meshTargetR = Objects.GetComponentInFamily<Renderer>(obj).gameObject.AddComponent<MeshTarget>();
			}
			
			if (obj.name.Contains("Player"))
				meshTargetR.OverrideFaceMaterial = DefaultMaterial;
			
			meshTargetR.SeparateMeshes = false; // true;
			
			return meshTargetR;
		}
		
        private void OnCreated(Info info, MeshCreationData cData)
        {
            EnableRigidbodies(cData);
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
            MeshCreation.CenterPivots(cData.CreatedObjects);

            RoomManager.sliceCount++;
			cutting = false;
			
			Debug.Log("cutting set to" + cutting);
        }

        private void EnableRigidbodies(MeshCreationData cData)
        {
            foreach(GameObject obj in cData.CreatedObjects)
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
	}
}
