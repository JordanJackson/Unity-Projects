using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class CollisionAudio : MonoBehaviour {

	public AudioClip impact;

	AudioSource musicSource;		// reference for updating volume
	AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
		musicSource = GameObject.FindGameObjectWithTag ("MusicManager").GetComponent<AudioSource> ();
	}

	void Update() {
		audioSource.volume = musicSource.volume;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.relativeVelocity.magnitude > 2) {
			audioSource.volume *= 0.2f;	// light impact
			audioSource.PlayOneShot (impact);
			audioSource.volume /= 0.2f;	// reset volume
		}
		if (coll.relativeVelocity.magnitude > 3) {
			audioSource.volume *= 0.4f;	// harder impact
			audioSource.PlayOneShot (impact);
			audioSource.volume /= 0.2f;	// reset volume
		}
	}
}
