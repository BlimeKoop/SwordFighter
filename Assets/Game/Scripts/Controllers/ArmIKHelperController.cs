using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIKHelperController : MonoBehaviour
{
	private Transform lowerChestBone;
	public Transform forearmBone;

    void Awake()
    {
		lowerChestBone = forearmBone.parent.parent.parent.parent.parent.parent;
        transform.position = TargetPosition();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition(), Time.deltaTime);
    }
	
	private Vector3 TargetPosition()
	{
		return transform.position = forearmBone.position - lowerChestBone.forward;
	}
}
