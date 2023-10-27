using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	private enum TargetPosition
	{
		Login,
		Selection,
		CreateRoom,
		JoinRandomRoom,
		RoomList,
		InsideRoom
	}
	
	public Transform positionParent;
	
	private TargetPosition targetPosition;
	private Quaternion rotationStore;
	private float moveDistance;
	
	private Dictionary<TargetPosition, Transform> positions = new Dictionary<TargetPosition, Transform>();
	
	public float speed = 7f;
	
	private bool moveDistanceSet;
	
    private void Start()
    {
        positions[TargetPosition.Login] = positionParent.GetChild(0);
        positions[TargetPosition.Selection] = positionParent.GetChild(1);
        positions[TargetPosition.CreateRoom] = positionParent.GetChild(2);
        positions[TargetPosition.JoinRandomRoom] = positionParent.GetChild(3);
        positions[TargetPosition.RoomList] = positionParent.GetChild(4);
        positions[TargetPosition.InsideRoom] = positionParent.GetChild(5);
		
		targetPosition = TargetPosition.Login;
    }
	
	private void LateUpdate()
	{
		if (!moveDistanceSet && transform.position != positions[targetPosition].position)
		{
			rotationStore = transform.rotation;
			
			moveDistance = Vector3.Distance(transform.position, positions[targetPosition].position);
			moveDistanceSet = true;
		}
		
		if (transform.position != positions[targetPosition].position)
		{
			transform.position = Vector3.Lerp(transform.position, positions[targetPosition].position, Time.deltaTime * speed);
			
			if (Vector3.Distance(transform.position, positions[targetPosition].position) < 0.001f)
			{
				transform.position = positions[targetPosition].position;
				transform.rotation = positions[targetPosition].rotation;
				
				moveDistanceSet = false;
			}
			else
			{
				float t = 1.0f - (Vector3.Distance(transform.position, positions[targetPosition].position) / moveDistance);
				t = Mathf.Pow(t, 4);
				
				transform.rotation = Quaternion.Lerp(rotationStore, positions[targetPosition].rotation, t);
			}
		}
	}

    public void OnLoginButtonClicked()
	{
		targetPosition = TargetPosition.Selection;
	}
	
    public void OnRoomListButtonClicked()
	{
		targetPosition = TargetPosition.RoomList;
	}
	
    public void OnBackButtonClicked()
	{
		targetPosition = TargetPosition.Selection;
	}

    public void OnCreateRoomButtonClicked()
	{
		targetPosition = TargetPosition.CreateRoom;
	}

    public void OnJoinRandomRoomButtonClicked()
	{
		targetPosition = TargetPosition.JoinRandomRoom;
	}

    public void OnStartGameButtonClicked()
	{
		targetPosition = TargetPosition.CreateRoom;
	}
}
