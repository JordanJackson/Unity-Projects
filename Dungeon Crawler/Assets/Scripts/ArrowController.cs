using UnityEngine;
using System.Collections;

// handles arrow damage and physics (sticking in to objects)
public class ArrowController : MonoBehaviour {

	public int damage;
	public float penetrationFactor = 0.1f;

	bool dud = false; 		// has this arrow already struck some object?
	GameObject stuckInto; 	// GO the arrow is currently stuck into
	Rigidbody rb;

	void Start() {
		rb = this.GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider other) {
		// given collider, if not the player and this arrow is not currently stuck
		if (other.tag != "Player" && stuckInto == null) {
			// if collider an enemy, and arrow not dud, handle dealing damage
			if (other.tag == "Enemy" && !dud) {
				EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
				if (enemyHealth != null) {
					enemyHealth.TakeDamage(damage);
				}

			}
			// update arrow state
			dud = true;
			rb.useGravity = false;
			rb.isKinematic = true;

			stuckInto = other.gameObject;
		}
	}

	void OnTriggerExit(Collider other) {
		// if arrow leaving stuckInto object for some reason, update state
		if (other.tag != "Player" && other.gameObject == stuckInto) {
			rb.isKinematic = false;
			rb.useGravity = true;
			stuckInto = null;
		}
	}
}
