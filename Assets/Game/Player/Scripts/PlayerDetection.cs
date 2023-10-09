using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection
{
    public static RaycastHit GroundHit(PlayerController playerController)
	{
		Vector3 originOffset = Vector3.up * (
			playerController.groundDetectionRadius + Mathf.Max(0.01f, playerController.groundStepUpDistance));
			
		if (!Physics.SphereCast(
			playerController.transform.position + originOffset,
			playerController.groundDetectionRadius,
			Vector3.down,
			out RaycastHit hit,
			originOffset.y - playerController.groundDetectionRadius + 0.1f,
			~(1 << 3)))
			return new RaycastHit();
		
		return hit;
	}
}
