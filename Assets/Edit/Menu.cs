using UnityEngine;
using System.Collections.Generic;
using Leap.Unity;
using Leap;

public class Menu : MonoBehaviour {
	LeapProvider provider;
	// Use this for initialization
	void Start () {
		provider = FindObjectOfType<LeapProvider>() as LeapProvider;
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = provider.CurrentFrame;
		List<Hand> hands = frame.Hands;
		foreach (Hand hand in hands) {
			if (hand.IsLeft) {
				transform.position = hand.PalmPosition.ToVector3() + hand.PalmNormal.ToVector3() * (transform.localScale.y * 0.1f + 0.02f);
				transform.rotation = hand.Basis.rotation.ToQuaternion();
			}
		}
	}
}
