using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    
	public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    //PlayerShooting playerShooting;
	Slider healthSlider;
	Image damageImage;
    bool isDead;
    bool damaged;


    void Awake ()
    {
        //playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
		GameObject canvas = GameObject.FindGameObjectWithTag ("UI Canvas");
		healthSlider = canvas.GetComponentInChildren<Slider> ();
		damageImage = canvas.GetComponentInChildren<Image> ();
    }


    void Update ()
    {
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;



        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        //playerShooting.DisableEffects ();
        //playerShooting.enabled = false;
    }


    public void RestartLevel ()
    {
        Application.LoadLevel (Application.loadedLevel);
    }
}
