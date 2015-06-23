using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioSource musicSource;
	public static MusicManager instance = null;

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		if (!musicSource.isPlaying) {
			musicSource.Play ();
		}
		DontDestroyOnLoad (gameObject);
	}
}
