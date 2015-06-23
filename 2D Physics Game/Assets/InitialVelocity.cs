using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class InitialVelocity : MonoBehaviour {

	public Vector3 initialVelocity;

	// Use this for initialization
	void Start () {
		this.GetComponent<Rigidbody2D>().velocity = initialVelocity;
	}

}
