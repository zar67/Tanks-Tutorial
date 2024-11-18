using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public Rigidbody Shell;                   // Prefab of the shell.
    public Transform FireTransform;           // A child of the tank where the shells are spawned.
    public AudioSource ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different ot the movement audio source.
    public float LaunchForce = 25f;           // The force given to the shell if the fire button is not held.

    private int m_playerNumber = 1;           // Used to identify the different players.
    private string m_fireButton;              // The input axis that is used for launching shells.

    private void Start()
    {
        TankMovement tankMovement = GetComponent<TankMovement>();
        if (tankMovement != null)
        {
            m_playerNumber = tankMovement.PlayerNumber;
        }

        // The fire axis is based on the player number.
        m_fireButton = "Fire" + m_playerNumber;
    }

    private void Update()
    {
        // If the fire button has just finished being pressed...
        if (Input.GetButtonUp(m_fireButton))
        {
            // ... launch the shell.
            Fire();
        }
    }

    private void Fire()
    {
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance = Instantiate(Shell, FireTransform.position, FireTransform.rotation);

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.linearVelocity = FireTransform.forward * LaunchForce;

        // Play the audio clip.
        ShootingAudio.Play();
    }
}