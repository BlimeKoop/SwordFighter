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
		
		bool initialized, syncingCuts, cutCompleted;
		
		private List<IEnumerator> currentCuts = new List<IEnumerator>();
		
		private void Start()
		{
			photonView = GetComponent<PhotonView>();
		}

		public void CutObject(string objectName, string[] rigidbodyNames, Vector3 localAxis, Vector3 localPoint)
		{
			if (!initialized)
				Start();

			Debug.Log("Going to cut " + objectName);
			
			IEnumeratorBox box = new IEnumeratorBox();
			box.numerator = CutObjectWhenReady(objectName, rigidbodyNames, localAxis, localPoint, box);
			
			currentCuts.Add(box.numerator);
			
			if (!syncingCuts)
				StartCoroutine(SyncCuts(objectName, rigidbodyNames, localAxis, localPoint));
		}
		
		private IEnumerator SyncCuts(string objectName, string[] rigidbodyNames,
		Vector3 localAxis, Vector3 localPoint)
		{
			syncingCuts = true;
			
			while (currentCuts.Count > 0)
			{
				cutCompleted = false;
				
				StartCoroutine(currentCuts[0]);
				
				yield return new WaitUntil(() => cutCompleted);
			}
			
			syncingCuts = false;
		}

		private IEnumerator CutObjectWhenReady(string objectName, string[] rigidbodyNames,
		Vector3 localAxis, Vector3 localPoint, IEnumeratorBox box)
		{
			Debug.Log(rigidbodyNames[0]);
			Debug.Log(rigidbodyNames[1]);
			
			// Wait until these have spawned in on this client
			yield return new WaitUntil(() => GameObject.Find(rigidbodyNames[0]) != null);
			yield return new WaitUntil(() => GameObject.Find(rigidbodyNames[1]) != null);
			
			GameObject obj = GameObject.Find(objectName);
			
			// Before going on to actually cutting the object check that some other player hasnt already cut/destroyed it
			// ( This is done after all waiting is completed oc )
			if (obj == null)
			{
				OnCut(false, null);
				yield break;
			}

			MeshTarget meshTarget = InitializeMeshTarget(obj);

			while (meshTarget == null)
				meshTarget = InitializeMeshTarget(obj);

			PlayerController playerController = meshTarget.GetComponentInParent<PlayerController>();
			
			if (playerController != null)
				playerController.Die();

			Debug.Log("Cutting " + obj.name);

            Cut(
				meshTarget,
				obj.transform.TransformPoint(localPoint),
				obj.transform.TransformDirection(localAxis).normalized,
				rigidbodyNames,
				OnCut, OnCreated, null,
				meshTarget.GetComponentInParent<PhotonView>());
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
			
			if (obj.layer == Collisions.PlayerLayer)
				meshTargetR.OverrideFaceMaterial = DefaultMaterial;
			
			meshTargetR.SeparateMeshes = false; // true; // Only two rigidbodies are spawned on the server beforehand
			
			return meshTargetR;
		}
		
		private void OnCut(bool success, Info info)
		{
			cutCompleted = true;
			currentCuts.RemoveAt(0);
		}
		
        private void OnCreated(Info info, MeshCreationData cData)
        {
            EnableRigidbodies(cData);
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
            MeshCreation.CenterPivots(cData.CreatedObjects);
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
			foreach(var IEn in currentCuts)
				StopCoroutine(IEn);
		}
	}
}
