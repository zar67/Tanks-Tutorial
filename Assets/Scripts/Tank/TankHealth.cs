using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float StartingHealth = 100f;               // The amount of health each tank starts with.
    public Slider Slider;                             // The slider to represent how much health the tank currently has.
    public Image FillImage;                           // The image component of the slider.
    public Color FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        
    private AudioSource m_explosionAudio;             // The audio source to play when the tank explodes.
    private ParticleSystem m_explosionParticles;      // The particle system the will play when the tank is destroyed.
    private float m_currentHealth;                    // How much health the tank currently has.
    private bool m_dead;                              // Has the tank been reduced beyond zero health yet?

    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        m_explosionAudio = m_explosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // When the tank is enabled, reset the tank's health and whether or not it's dead.
        m_currentHealth = StartingHealth;
        m_dead = false;

        // Update the health slider's value and color.
        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        m_currentHealth -= amount;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_currentHealth <= 0f && !m_dead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        Slider.value = m_currentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        FillImage.color = Color.Lerp(ZeroHealthColor, FullHealthColor, m_currentHealth / StartingHealth);
    }

    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        m_dead = true;

        // Move the instantiated explosion prefab to the tank's position and turn it on.
        m_explosionParticles.transform.position = transform.position;
        m_explosionParticles.gameObject.SetActive(true);

        // Play the particle system of the tank exploding.
        m_explosionParticles.Play();

        // Play the tank explosion sound effect.
        m_explosionAudio.Play();

        // Turn the tank off.
        gameObject.SetActive(false);
    }
}