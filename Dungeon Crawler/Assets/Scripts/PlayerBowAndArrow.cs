using UnityEngine;
using System.Collections;

public class PlayerBowAndArrow : MonoBehaviour {

	public GameObject arrowPrefab;
	public GameObject projectileSpawnPoint;
	GameObject arrowCollector;

	public float arrowSpeed = 100f;
	public float fireRate = 2.0f;

	void Start() {
		arrowCollector = GameObject.Find ("ArrowCollector");
	}

	public void Shoot () {
		GameObject arrow = (GameObject)Instantiate (arrowPrefab, projectileSpawnPoint.transform.position, projectileSpawnPoint.transform.rotation);
		arrow.transform.parent = arrowCollector.transform;
		arrow.GetComponent<Rigidbody>().AddForce (projectileSpawnPoint.transform.forward * arrowSpeed);
	}
}
