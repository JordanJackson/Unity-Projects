using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour {

	// References to required GameObjects
	AudioSource audioSource;
	Slider volumeSlider;
	Text volumeText;

	int volumeDisplay = 100;		// Human-readable value used to display current level
	float volumeValue;				// actual value updated and passed to MusicManager

	// Use this for initialization
	void Start () {
		audioSource = GameObject.FindGameObjectWithTag ("MusicManager").GetComponent<AudioSource> ();
		volumeSlider = GetComponent<Slider> ();
		if (audioSource.volume < 1.0f)
			volumeSlider.value = audioSource.volume;	// ensure previous volume is retained
		
		volumeText = GetComponentInChildren<Text> ();
		volumeValue = volumeSlider.value;
		volumeDisplay = (int)(100 * volumeValue);
		volumeText.text = "Volume: " + volumeDisplay;
		audioSource.volume = volumeValue;
		volumeSlider.onValueChanged.AddListener (UpdateVolume);
	}
	
	public void UpdateVolume(float value) {
		volumeValue = value;
		volumeDisplay = (int)(100 * volumeValue);
		volumeText.text = "Volume: " + volumeDisplay;
		audioSource.volume = volumeValue;
	}
}
