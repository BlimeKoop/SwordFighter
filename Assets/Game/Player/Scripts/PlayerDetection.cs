using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection
{
    public static RaycastHit GroundHit(PlayerController playerController)
	{
		Vector3 originOffset = Vector3.up * (
			playerController.groundDetectionRadius + Mathf.Max(0.01f, playerController.groundStepUpDistance));
			
		float distance = originOffset.y - playerController.groundDetectionRadius + 0.1f;
			
		if (!Physics.SphereCast(
			playerController.transform.position + originOffset,
			playerController.groundDetectionRadius,
			Vector3.down,
			out RaycastHit hit,
			distance,
			~(1 << Collisions.PlayerLayer)))
		{
			return new RaycastHit();
		}
		
		return hit;
	}
}
