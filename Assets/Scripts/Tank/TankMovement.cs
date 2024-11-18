using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float Speed = 12f;                   // How fast the tank moves forward and back.
    public float TurnSpeed = 180f;              // How fast the tank turns in degrees per second.
    public AudioSource MovementAudio;           // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip EngineIdling;              // Audio to play when the tank isn't moving.
    public AudioClip EngineDriving;             // Audio to play when the tank is moving.
	public float PitchRange = 0.2f;             // The amount by which the pitch of the engine noises can vary.

    private static int m_tankCount = 1;
    private int m_playerNumber = 1;                // Used to identify which tank belongs to which player.
    private string m_movementAxisName;          // The name of the input axis for moving forward and back.
    private string m_turnAxisName;              // The name of the input axis for turning.
    private Rigidbody m_rigidbody;              // Reference used to move the tank.
    private float m_movementInputValue;         // The current value of the movement input.
    private float m_turnInputValue;             // The current value of the turn input.
    private float m_originalPitch;              // The pitch of the audio source at the start of the scene.
    private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

    public int PlayerNumber => m_playerNumber;

    // Called at the very start of the game when the script is setup.
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_playerNumber = m_tankCount;
        m_tankCount++;
    }

    // Called whenever the gameobject this component is attatched to is turned on.
    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_rigidbody.isKinematic = false;

        // Also reset the input values.
        m_movementInputValue = 0f;
        m_turnInputValue = 0f;

        // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
        // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
        // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Play();
        }
    }

    // Called whenever the gameobject this component is attatched to  is turned off.
    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_rigidbody.isKinematic = true;

        // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
        for(int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Stop();
        }
    }

    // Called after all script have had their Awake function called.
    private void Start()
    {
        // The axes names are based on player number.
        m_movementAxisName = "Vertical" + m_playerNumber;
        m_turnAxisName = "Horizontal" + m_playerNumber;

        // Store the original pitch of the audio source.
        m_originalPitch = MovementAudio.pitch;
    }

    // Called every frame of the game.
    // For a game running at 60FPS which would be called 60 times a second.
    private void Update()
    {
        // Store the value of both input axes.
        m_movementInputValue = Input.GetAxis(m_movementAxisName);
        m_turnInputValue = Input.GetAxis(m_turnAxisName);

        SetCorrectEngineAudio();
    }

    // Called event fixed-frame rate.
    // Similar to Update but mainly used for physics.
    private void FixedUpdate()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
        Turn();
    }

    private void SetCorrectEngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs (m_movementInputValue) < 0.1f && Mathf.Abs(m_turnInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (MovementAudio.clip == EngineDriving)
            {
                // ... change the clip to idling and play it.
                MovementAudio.clip = EngineIdling;
                MovementAudio.pitch = Random.Range(m_originalPitch - PitchRange, m_originalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (MovementAudio.clip == EngineIdling)
            {
                // ... change the clip to driving and play.
                MovementAudio.clip = EngineDriving;
                MovementAudio.pitch = Random.Range(m_originalPitch - PitchRange, m_originalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
    }

    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_movementInputValue * Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_rigidbody.MovePosition(m_rigidbody.position + movement);
    }

    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_turnInputValue * TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        var turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_rigidbody.MoveRotation(m_rigidbody.rotation * turnRotation);
    }
}