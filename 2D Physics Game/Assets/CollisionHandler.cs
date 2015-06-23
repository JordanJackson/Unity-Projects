using UnityEngine;
using System.Collections;

public class CollisionHandler : MonoBehaviour {

	GameManager gameManager;

	void Start() {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void OnCollisionEnter2D() {
		if (Camera.main.transform.position.y < transform.position.y) {
			gameManager.UpdateTargetHeight(transform.position.y);
		}
	}
}
