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

        bool ready;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();

            ready = true;
        }

        [PunRPC]
		public void CutObject(string objectName, Vector3 cutAxis, Vector3 point)
        {
            if (!ready)
                Start();

            // Debug.Log("Going to cut " + objectName);

            StartCoroutine(CutObjectWhenReady(objectName, cutAxis, point));
        }

		private IEnumerator CutObjectWhenReady(string objectName, Vector3 cutAxis, Vector3 point)
		{
            // If i'm not the client instantiating the rigidbodies on the server
            if (!photonView.IsMine)
            {
                string searchFor = $"{objectName} {RoomManager.sliceCount} (1/";

                // Should probably wait until they're spawned
                yield return new WaitUntil(() => FoundObject(searchFor));
            }

            Debug.Log($"Cutting {objectName}");

            MeshTarget meshTarget = InitializeMeshTarget(GameObject.Find(objectName));

			PlayerController playerController = meshTarget.GetComponentInParent<PlayerController>();
			
			if (playerController != null)
				playerController.Die();

            Cut(meshTarget, point, cutAxis.normalized, null, OnCreated, null, meshTarget.GetComponent<PhotonView>());
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
			MeshTarget meshTargetR = obj.AddComponent<MeshTarget>();
			
			if (obj.name.Contains("Player"))
				meshTargetR.OverrideFaceMaterial = DefaultMaterial;
			
			meshTargetR.GameobjectRoot = obj;
			meshTargetR.SeparateMeshes = false; // true;
			
			return meshTargetR;
		}
		
        private void OnCreated(Info info, MeshCreationData cData)
        {
            EnableRigidbodies(cData);
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
            MeshCreation.CenterPivots(cData.CreatedObjects);

            RoomManager.sliceCount++;
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
