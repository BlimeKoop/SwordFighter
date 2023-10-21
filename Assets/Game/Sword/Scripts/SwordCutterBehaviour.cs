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
		
		private List<IEnumerator> runningIEnumerators = new List<IEnumerator>();
		
		private void Start()
		{
			photonView = GetComponent<PhotonView>();
		}

		public void CutObject(string objectName, string[] rigidbodyNames, Vector3 cutAxis, Vector3 point)
		{
			if (!initialized)
				Start();

			Debug.Log("Going to cut " + objectName);
			
			IEnumeratorBox box = new IEnumeratorBox();
			
			IEnumerator cutObjectWhenReady = CutObjectWhenReady(objectName, rigidbodyNames, cutAxis, point, box);
			
			box.numerator = cutObjectWhenReady;
			StartCoroutine(cutObjectWhenReady);
		}

		private IEnumerator CutObjectWhenReady(string objectName, string[] rigidbodyNames,
		Vector3 cutAxis, Vector3 point, IEnumeratorBox box)
		{
			runningIEnumerators.Add(box.numerator);
			
			// This client may still be cutting something so wait if that's the case
			yield return new WaitWhile(() => cutting);
			
			cutting = true;
			
			GameObject obj = GameObject.Find(objectName);
			
			// Wait until these have spawned in on this client
			yield return new WaitUntil(() => GameObject.Find(rigidbodyNames[0]) != null);
			yield return new WaitUntil(() => GameObject.Find(rigidbodyNames[1]) != null);

			MeshTarget meshTarget = InitializeMeshTarget(obj);

			while (meshTarget == null)
				meshTarget = InitializeMeshTarget(obj);

			PlayerController playerController = meshTarget.GetComponentInParent<PlayerController>();
			
			if (playerController != null)
				playerController.Die();

			Debug.Log("Cutting " + obj.name);

            Cut(meshTarget, point, cutAxis.normalized, rigidbodyNames,
			null, OnCreated, null, meshTarget.GetComponentInParent<PhotonView>());
			
			runningIEnumerators.Remove(box.numerator);
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
		
		public void StopCoroutines()
		{
			foreach(var IEn in runningIEnumerators)
				StopCoroutine(IEn);
		}
	}
}
