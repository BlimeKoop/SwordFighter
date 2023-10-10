using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicMeshCutter
{
	public class SwordCutterBehaviour : CutterBehaviour
	{
		public void CutObject(GameObject obj, GameObject swordObj, Vector3 swordVelocity)
		{
			// Debug.Log("In Cut(), obj = " + obj);			
		
			if (obj.GetComponent<Renderer>() == null && obj.transform.childCount > 0)
				CutObject(obj.transform.GetChild(0).gameObject, swordObj, swordVelocity);
		
			if (obj.GetComponent<Renderer>() == null)
				return;

			MeshTarget meshTarget = InitializeMeshTarget(obj);

			Vector3 cutPlane = Vector3.Cross(swordVelocity, swordObj.transform.forward).normalized;
			
			Debug.Log($"Cutting {obj}");	
			
			Cut(meshTarget, swordObj.transform.position, cutPlane, null, OnCreated);
		}

		private MeshTarget InitializeMeshTarget(GameObject obj)
		{			
			MeshTarget meshTargetR = obj.AddComponent<MeshTarget>();
			
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
