using UnityEngine;
using System.Collections;

public class PlayerAttackController : MonoBehaviour {
	
	PlayerBowAndArrow bowScript;
	string selectedWeapon;
	float timer;

	// Use this for initialization
	void Start () {
		bowScript = GetComponent<PlayerBowAndArrow> ();
		selectedWeapon = "Bow";
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			selectedWeapon = "Sword";
			// update UI
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			selectedWeapon = "Bow";
			// update UI
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			selectedWeapon = "Magic";
			// update UI
		}

		switch (selectedWeapon) {
			case "Bow":
			{
				if (Input.GetButton ("Fire1") && timer >= bowScript.fireRate) {
					bowScript.Shoot ();
					timer = 0f;
				}
			break;
			}
		}
	}
}
