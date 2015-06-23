using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseManager : MonoBehaviour {

	public LineRenderer dragLine;
	public bool useSpring = false;
	public float dragSpeed = 20f;

	Rigidbody2D grabbedObject = null;
	SpringJoint2D springJoint = null;

	void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (Input.GetMouseButtonDown(0)) {

			// We clicked, but on what?
			Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 direction = Vector2.zero;

			RaycastHit2D hit = Physics2D.Raycast (clickPosition, direction);
			if (hit.collider != null) {
				// We clicked on something that has a collider
				if (hit.collider.attachedRigidbody != null) {
					grabbedObject = hit.collider.attachedRigidbody;

					if (useSpring) {
						springJoint = grabbedObject.gameObject.AddComponent<SpringJoint2D>();
						springJoint.connectedBody = null;		// one of these two lines
						springJoint.enableCollision = true;	// required to wake SpringJoint2D
						springJoint.anchor = grabbedObject.transform.InverseTransformPoint(hit.point);	// take world point and convert to local coordinates
						springJoint.connectedAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						springJoint.distance = 0.1f;
						springJoint.dampingRatio = 1;
						springJoint.frequency = 1;
					}
					else {
						// using velocity instead
						grabbedObject.gravityScale = 0;

					}
					dragLine.enabled = true;
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if (grabbedObject != null) {
				if (useSpring) {
					Destroy(springJoint);
				}
				else {
					grabbedObject.gravityScale = 1;
				}
				grabbedObject = null;
				dragLine.enabled = false;
			}
		}

		if (useSpring && springJoint != null) {
			springJoint.connectedAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		if (Input.touchCount > 0) {
			Touch currentTouch = Input.touches[0];

			if (currentTouch.phase == TouchPhase.Began) {
				// We clicked, but on what?
				Vector2 clickPosition = Camera.main.ScreenToWorldPoint(currentTouch.position);
				Vector2 direction = Vector2.zero;
				
				RaycastHit2D hit = Physics2D.Raycast (clickPosition, direction);
				if (hit.collider != null) {
					// We clicked on something that has a collider
					
					if (hit.collider.attachedRigidbody != null) {
						grabbedObject = hit.collider.attachedRigidbody;
						
						if (useSpring) {
							springJoint = grabbedObject.gameObject.AddComponent<SpringJoint2D>();
							springJoint.connectedBody = null;		// one of these two lines
							springJoint.collideConnected = true;	// required to wake SpringJoint2D
							springJoint.anchor = grabbedObject.transform.InverseTransformPoint(hit.point);	// take world point and convert to local coordinates
							springJoint.connectedAnchor = Camera.main.ScreenToWorldPoint(currentTouch.position);
							springJoint.distance = 0.1f;
							springJoint.dampingRatio = 1;
							springJoint.frequency = 1;
						}
						else {
							// using velocity instead
							grabbedObject.gravityScale = 0;
							
						}
						dragLine.enabled = true;
					}
				}
			}

			if (currentTouch.phase == TouchPhase.Ended) {
				if (grabbedObject != null) {
					if (useSpring) {
						Destroy(springJoint);
					}
					else {
						grabbedObject.gravityScale = 1;
					}
					grabbedObject = null;
					dragLine.enabled = false;
				}
			}

			if (useSpring && springJoint != null) {
				springJoint.connectedAnchor = Camera.main.ScreenToWorldPoint(currentTouch.position);
			}
		}
#endif
	}

	void LateUpdate() {
		if (grabbedObject != null) {
			if (useSpring) {
				Vector3 worldAnchor = grabbedObject.transform.TransformPoint(springJoint.anchor);
				dragLine.SetPosition(0, new Vector3(worldAnchor.x, worldAnchor.y, -1));
				dragLine.SetPosition(1, new Vector3(springJoint.connectedAnchor.x, springJoint.connectedAnchor.y, -1));
			}
			else {
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				dragLine.SetPosition (0, new Vector3(grabbedObject.position.x, grabbedObject.position.y, -1));
				dragLine.SetPosition (1, new Vector3(mousePosition.x, mousePosition.y, -1));
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
				Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
				dragLine.SetPosition(0, new Vector3(grabbedObject.position.x, grabbedObject.position.y, -1));
				dragLine.SetPosition (1, new Vector3(touchPosition.x, touchPosition.y, -1));
#endif
			}
		}
	}

	void FixedUpdate() {
		if (grabbedObject != null) {
			if (useSpring) {

			}
			else {
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
				// Move the object with the mouse
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				Vector2 direction = mousePosition - grabbedObject.position;
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
				Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
				Vector2 direction = touchPos - grabbedObject.position;
#endif
				grabbedObject.velocity = direction * dragSpeed;
			}
		}
	}
}
