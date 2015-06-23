using UnityEngine;
using System.Collections;

public class BoxLauncher : MonoBehaviour {

	public GameObject[] shapePrefabs;

	public float woodChance = 0.5f;
	public float stoneChance = 0.3f;
	public float metalChance = 0.15f;
	public float glassChance = 0.05f;

	public float boxChance = 0.4f;
	public float tallBoxChance = 0.3f;
	public float triangleChance = 0.2f;
	public float circleChance = 0.1f;

	public float fireDelay = 3f;
	public float fireVelocity = 10f;
	public float heightFactor = 2f;
	public float nextFire = 1f;

	ScoreManager scoreManager;
	GameManager gameManager;

	// Use this for initialization
	void Start () {
		scoreManager = GameObject.FindObjectOfType<ScoreManager> ();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
	}

	void FixedUpdate () {
		nextFire -= Time.deltaTime;
		if (nextFire <= 0 && !scoreManager.gameOver) {
			// Spawn new object
			nextFire = fireDelay;

			GameObject boxGO = 
				Instantiate(
					shapePrefabs[Random.Range (0, shapePrefabs.Length)],
					transform.position,
					transform.rotation)
					as GameObject;

			boxGO.GetComponent<Rigidbody2D>().velocity = transform.rotation * new Vector2(0, fireVelocity + (gameManager.targetHeight * heightFactor));
			scoreManager.ObjectShot();
		}
	}
}
