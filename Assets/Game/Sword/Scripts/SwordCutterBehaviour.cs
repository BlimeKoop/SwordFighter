using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicMeshCutter
{
	public class SwordCutterBehaviour : CutterBehaviour
	{
		public void CutObject(GameObject obj, PlayerSwordController swordController, Collision col)
		{
			// Debug.Log("In Cut(), obj = " + obj);			
			
			Transform rollController = swordController.rollController;
		
			if (obj.GetComponent<Renderer>() == null && obj.transform.childCount > 0)
				CutObject(obj.transform.GetChild(0).gameObject, swordController, col);
		
			if (obj.GetComponent<Renderer>() == null)
				return;

			MeshTarget meshTarget = InitializeMeshTarget(obj);

			Vector3 cross = Vectors.SafeCross(Vector3.up, col.relativeVelocity);
			Vector3 cutPlane = Vector3.Cross(col.relativeVelocity, cross).normalized;
			
			// Debug.Log($"Cutting {obj}");	
			
			Cut(meshTarget, col.GetContact(0).point, cutPlane, null, OnCreated);
		}

		private MeshTarget InitializeMeshTarget(GameObject obj)
		{			
			MeshTarget meshTargetR = obj.AddComponent<MeshTarget>();
			
			if (obj.name.Contains("Player"))
				meshTargetR.OverrideFaceMaterial = DefaultMaterial;
			
			meshTargetR.GameobjectRoot = obj;
			meshTargetR.SeparateMeshes = false;
			
			return meshTargetR;
		}
		
        private void OnCreated(Info info, MeshCreationData cData)
        {
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
			MeshCreation.CenterPivots(cData.CreatedObjects);
        }
	}
}
