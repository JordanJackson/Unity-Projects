using UnityEngine;
using System.Collections;

public class ObjectKiller : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (transform.position.y < -10) {
			Kill();
		}
	}

	void Kill() {
		//GameObject.FindObjectOfType<ScoreManager> ().ObjectFell ();
		GameObject.FindObjectOfType<ScoreManager> ().GameOver ();
		Destroy (this);
	}
}
