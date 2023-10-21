using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDeltaTracker : MonoBehaviour {
	
		public enum UpdateMode {

		FixedUpdate,
		Update,
		LateUpdate
	}

	[SerializeField]
	private UpdateMode _UpdateMode = UpdateMode.Update;
	
	private Vector3 lastPosition;
	private Vector3 _positionDelta; public Vector3 positionDelta { get { return _positionDelta; } }
	
	
    private void Start() {
		
        lastPosition = transform.position;
    }

    private void FixedUpdate() {
        
		if ( _UpdateMode != UpdateMode.FixedUpdate )
			return;
		
		TrackDelta();
    }

    private void Update() {
        
		if ( _UpdateMode != UpdateMode.Update )
			return;

		TrackDelta();
    }

    private void LateUpdate() {
        
		if ( _UpdateMode != UpdateMode.LateUpdate )
			return;
		
		TrackDelta();
    }
	
	private void TrackDelta()
	{
		_positionDelta = transform.position - lastPosition;
		lastPosition = transform.position;
	}
}
