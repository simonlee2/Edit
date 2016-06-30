using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using Leap.Unity;

public class PinchCreate : MonoBehaviour {

	[SerializeField]
	private PinchDetector _pinchDetector_L;
	[SerializeField]
	private PinchDetector _pinchDetector_R;

	private GameObject plane;
	private Vector3 prevVector;
	private Transform _anchor;

	// Use this for initialization
	void Start () {
		
	}

	const float ANGLE = 1.0f/6.0f * Mathf.PI;
	
	// Update is called once per frame
	void Update () {

		if ((_pinchDetector_L.DidStartPinch && _pinchDetector_R.IsPinching) || (_pinchDetector_R.DidStartPinch && _pinchDetector_L.IsPinching)) {
			
			// Instantiate prefab between fingers

			//plane.transform.rotation = InputTracking.GetLocalRotation (VRNode.Head);
			//Debug.Log(string.Format("Head Rotation: {0}", InputTracking.GetLocalRotation(VRNode.Head), this.plane));
			//Debug.Log (string.Format ("Plane Rotation: {0}", plane.transform.rotation), this.plane);
			//Debug.Log (string.Format ("Plane local rotation: {0}", plane.transform.localRotation), this.plane);

			//var axis = Camera.main.transform.rotation * InputTracking.GetLocalRotation(VRNode.Head) * Vector3.forward;
			//var rotation = Quaternion.AngleAxis (-30, axis);
			//plane.transform.rotation = rotation;

			//plane.transform.localScale = plane.transform.localScale * 0.1f;
			this.plane = createPlane();
			var vec = _pinchDetector_L.Position - _pinchDetector_R.Position;
			this.prevVector = vec;
		}

		if (_pinchDetector_L.DidEndPinch || _pinchDetector_R.DidEndPinch) {
			// Drop attachment of prefab
//				plane.AddComponent<Rigidbody> ();
//				var rigid = plane.GetComponent<Rigidbody> ();
//				rigid.useGravity = false;
			this.plane = null;
		}

		if (_pinchDetector_L.IsPinching && _pinchDetector_R.IsPinching) {
			// Make prefab follow the posistions of the pinches
			if (this.plane != null) {
				plane.transform.SetParent (null, true);
				// Set position
				plane.transform.position = (_pinchDetector_L.Position + _pinchDetector_R.Position) / 2.0f;
				_anchor.position = plane.transform.position;
				// Feature flag
				var myVersion = false;

				// Set vector
				if (myVersion) {
					var vec = _pinchDetector_L.Position - _pinchDetector_R.Position;
					vec /= vec.magnitude;
					var rotation = findRotation (prevVector, vec);
					prevVector = vec;
					plane.transform.rotation *= rotation;
				} else {
					Quaternion pp = Quaternion.Lerp (_pinchDetector_L.Rotation, _pinchDetector_R.Rotation, 0.5f);
					Vector3 u = pp * Vector3.up;
					plane.transform.LookAt (_pinchDetector_L.Position, u);
					plane.transform.Rotate (new Vector3 (45, 90, 0));

					Debug.Log (string.Format ("L.rotation {0}, R.Rotation {1}, pp {2}, u {3}, plane.rotation {2}", _pinchDetector_L.Rotation, _pinchDetector_R.Rotation, pp, u, plane.transform.rotation));
				}

				// Set scale
				var length = Vector3.Distance(_pinchDetector_L.Position, _pinchDetector_R.Position);
				var width = length * Mathf.Cos (ANGLE);
				var height = Mathf.Sqrt (Mathf.Pow(length, 2) - Mathf.Pow(width, 2));
				plane.transform.localScale = new Vector3 (width, height, 0.01f);

				plane.transform.SetParent (_anchor, true);
			}
		}
	}

	Transform anchor(GameObject gameObject) {
		GameObject pinchControl = new GameObject("RTS Anchor");
		pinchControl.transform.parent = gameObject.transform.parent;
		gameObject.transform.parent = pinchControl.transform;
		return pinchControl.transform;
	}

	GameObject createPlane() {
		var plane = Instantiate (Resources.Load ("Cube")) as GameObject;
		_anchor = anchor (plane);
		plane.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		return plane;
	}

	Quaternion findRotation(Vector3 a, Vector3 b) {
		var axis = Vector3.Cross (a, b);
		axis = axis.normalized;

		var angle = Vector3.Dot (a, b) / (a.magnitude * b.magnitude);
		return Quaternion.AngleAxis (Mathf.Acos (angle) * 180 / Mathf.PI, axis);
	}
	
}
