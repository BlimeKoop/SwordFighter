using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{	
    private PlayerController playerController;
	
	private Transform sword;
	private Camera cam;
	
	private Transform armBone;
	private Transform midSectionBone;
	
    [SerializeField][Range(0, 1)]
	private float grabPointRatio = 0.25f;
	
    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
		sword = playerController.GetSwordModel().transform;
		
		cam = Camera.main;
		
		armBone = transform.parent.parent;
		midSectionBone = transform.parent.parent.parent.parent.parent.parent;
    }

    private void LateUpdate()
    {
		
    }
}
