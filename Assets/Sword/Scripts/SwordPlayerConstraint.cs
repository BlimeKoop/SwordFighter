using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerConstraint : MonoBehaviour
{
	public Rigidbody playerRB;
	
	private Vector3 playerPositionStore;
	private Vector3 playerPositionOffset;

	private bool ready;

	private void FixedUpdate()
	{
		if (!ready)
		{
			playerPositionStore = playerRB.position;
			
			ready = true;
			return;
		}
		
		ComparePlayerTransformData();
		RecordPlayerTransformData();
	}
	
	public void RecordPlayerTransformData() { playerPositionStore = playerRB.position; }
	public void ComparePlayerTransformData() {  playerPositionOffset = playerRB.position - playerPositionStore; }
	
	public Vector3 GetPlayerPositionOffset() { return playerPositionOffset; }
}
