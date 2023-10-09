using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{	
    private PlayerController playerController;
	private Camera camera;
	
	private Transform armBone;
	private Transform midSectionBone;
	
    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		camera = playerController.camera.GetComponent<Camera>();
		
		armBone = transform.parent.parent;
		midSectionBone = transform.parent.parent.parent.parent.parent.parent;
    }

    private void LateUpdate()
    {
		
    }
}
