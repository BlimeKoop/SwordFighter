using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DynamicMeshCutter
{
	public class SwordCutterBehaviour : CutterBehaviour
	{
		private PhotonView photonView;

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
        }

        [PunRPC]
		public void CutObject(string objectName, Vector3 relativeVlocity, Vector3 point)
		{
			GameObject obj = GameObject.Find(objectName);
            // Debug.Log("In Cut(), obj = " + obj);

			MeshTarget meshTarget = InitializeMeshTarget(obj);

			Vector3 cross = Vectors.SafeCross(Vector3.up, relativeVlocity);
			Vector3 cutPlane = Vector3.Cross(relativeVlocity, cross).normalized;
			
			// Debug.Log($"Cutting {obj}");	
			
			Cut(meshTarget, point, cutPlane, null, OnCreated);
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
			MeshCreation.MakeNamesUnique(cData.CreatedObjects, photonView);
            // MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
            // MeshCreation.CenterPivots(cData.CreatedObjects);
        }
	}
}
