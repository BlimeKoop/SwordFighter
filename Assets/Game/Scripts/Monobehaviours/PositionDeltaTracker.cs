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
	private UpdateMode updateMode = UpdateMode.Update;
	
	private Vector3 lastPosition;
	private Vector3 _delta; public Vector3 delta { get { return _delta; } }
	private Vector3 _safeDelta; public Vector3 safeDelta { get { return _safeDelta; } }
	
	
    private void Start() {
		
        lastPosition = transform.position;
    }

    private void FixedUpdate() {
        
		if (updateMode != UpdateMode.FixedUpdate )
			return;
		
		TrackDelta();
    }

    private void Update() {
        
		if (updateMode != UpdateMode.Update )
			return;

		TrackDelta();
    }

    private void LateUpdate() {
        
		if (updateMode != UpdateMode.LateUpdate )
			return;
		
		TrackDelta();
    }
	
	private void TrackDelta()
	{
		_delta = transform.position - lastPosition;
		
		float safeThreshold = 0.2f;
		
		if (updateMode == UpdateMode.FixedUpdate)
			safeThreshold *= Time.fixedDeltaTime;
		else
			safeThreshold *= Time.deltaTime;

		if (_delta.magnitude > safeThreshold)
			_safeDelta = _delta;

		lastPosition = transform.position;
	}
}
