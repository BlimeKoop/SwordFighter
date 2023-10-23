using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection
{
    public static bool GroundHit(PlayerController playerController, out RaycastHit hit, out float groundDistance)
	{
		hit = new RaycastHit();
		groundDistance = 10f;
		
		Vector3 originOffset = Vector3.up * (
			playerController.groundDetectionRadius + Mathf.Max(0.01f, playerController.groundStepUpDistance));
			
		if (playerController.crouching)
			originOffset /= 3;
			
		float maxDistance = originOffset.y - playerController.groundDetectionRadius + 0.15f;
		
		RaycastHit[] hits = Physics.SphereCastAll(
			playerController.transform.position + originOffset,
			playerController.groundDetectionRadius,
			Vector3.down,
			10f);
		
		foreach(RaycastHit h in hits)
		{
			if (h.transform.gameObject.layer == Collisions.PlayerLayer && h.transform.gameObject == playerController.gameObject)
				continue;
			
			if (h.transform.gameObject.layer == Collisions.SwordLayer && h.transform.gameObject == playerController.sword)
				continue;
			
			hit = h;
			break;
		}
		
		if (hits.Length == 0 ||
			playerController.disableGround ||
			hit.distance > maxDistance ||
			hit.collider.transform == playerController.swordModel)
		{
			if (hit.transform != null)
				groundDistance = hit.distance - Mathf.Abs(originOffset.y);
			
			return false;
		}
		
		groundDistance =  hit.distance - Mathf.Abs(originOffset.y);
		
		return true;
	}
}
