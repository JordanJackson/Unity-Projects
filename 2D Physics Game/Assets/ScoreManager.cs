using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public int score;
	public int dropsLeft = 5;
	public int objectGain = 10;
	public int objectLoss = 5;

	// UI elements
	public Text scoreText;
	public Text gameOverText;
	public Text introText;
	public Image gameOverImage;
	public Button resetButton;

	public bool gameOver;

	// Use this for initialization
	void Start () {
		gameOver = false;
		scoreText.text = "Score: " + score;
		gameOverText.gameObject.SetActive (false);
		gameOverImage.gameObject.SetActive (false);
		resetButton.gameObject.SetActive (false);
		//dropsText.text = "Drops Allowed: " + dropsLeft;
		Invoke ("DisableIntroText", 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "Score: " + score;
		//dropsText.text = "Drops Allowed: " + dropsLeft;

	}

	void DisableIntroText() {
		introText.gameObject.SetActive (false);
	}

	public void GameOver() {
		gameOver = true;
		gameOverText.gameObject.SetActive (true);
		gameOverImage.gameObject.SetActive (true);
		resetButton.gameObject.SetActive (true);
		resetButton.interactable = true;
		resetButton.onClick.AddListener (() => Application.LoadLevel (Application.loadedLevel));
	}

	public void ObjectShot() {
		score += objectGain;
	}

	public void ObjectFell() {
		score -= objectLoss;
	}
}
