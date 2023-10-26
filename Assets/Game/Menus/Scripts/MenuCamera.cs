using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	private enum FacePosition
	{
		Login,
		Selection,
		CreateRoom,
		JoinRandomRoom,
		RoomList,
		InsideRoom
	}
	
	public Transform facePositionParent;
	
	private FacePosition facePosition;
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	
	private Dictionary<FacePosition, Transform> positions = new Dictionary<FacePosition, Transform>();
	
	public float speed = 7f;
	
    private void Start()
    {
        positions[FacePosition.Login] = facePositionParent.GetChild(0);
        positions[FacePosition.Selection] = facePositionParent.GetChild(1);
        positions[FacePosition.CreateRoom] = facePositionParent.GetChild(2);
        positions[FacePosition.JoinRandomRoom] = facePositionParent.GetChild(3);
        positions[FacePosition.RoomList] = facePositionParent.GetChild(4);
        positions[FacePosition.InsideRoom] = facePositionParent.GetChild(5);
		
		facePosition = FacePosition.Login;
    }
	
	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, positions[facePosition].position, Time.deltaTime * speed);
		transform.rotation = Quaternion.Lerp(transform.rotation, positions[facePosition].rotation, Time.deltaTime * speed);
	}

    public void OnLoginButtonClicked()
	{
		facePosition = FacePosition.Selection;
	}
	
    public void OnRoomListButtonClicked()
	{
		facePosition = FacePosition.RoomList;
	}
	
    public void OnBackButtonClicked()
	{
		facePosition = FacePosition.Selection;
	}

    public void OnCreateRoomButtonClicked()
	{
		facePosition = FacePosition.CreateRoom;
	}

    public void OnJoinRandomRoomButtonClicked()
	{
		facePosition = FacePosition.JoinRandomRoom;
	}

    public void OnStartGameButtonClicked()
	{
		facePosition = FacePosition.CreateRoom;
	}
}
