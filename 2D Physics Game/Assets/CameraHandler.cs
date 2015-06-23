using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

	GameManager gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		float targetHeight = gameManager.targetHeight;
		if (transform.position.y < targetHeight) {
			Vector3 toYPosition = transform.position;
			toYPosition.y = targetHeight;
			float toSize = targetHeight + 3f;
			Vector3 tempPos = transform.position;
			tempPos.y = Mathf.Lerp (transform.position.y, toYPosition.y, 0.1f);
			Camera.main.transform.position = tempPos;
			float currentSize = Camera.main.orthographicSize;
			Camera.main.orthographicSize = Mathf.Lerp (currentSize, toSize, 0.1f);
		}
	}
}
