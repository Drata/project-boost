using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    // components
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float force = 10f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float loadDelay;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip levelFinished;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelFinishedParticles;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;
    bool isCollisionOn = true;
  

    // Start is called before the first frame update
    void Start()
    {
        // Caches components
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
        
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // If we are dead don't process collisions
        if (state != State.Alive || !isCollisionOn)
        {
            return;
        }

        switch(collision.gameObject.tag)
        { 
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence(collision.gameObject);
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence(GameObject landingPlatform)
    {
        Oscillator os = landingPlatform.gameObject.GetComponent<Oscillator>();
        if (os) { os.Stop(); }
        state = State.Transcending;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(levelFinished);
        levelFinishedParticles.Play();
        Invoke("LoadLevel", loadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadLevel", loadDelay); 
    }

    private void LoadLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int nextLevel = currentLevel + 1;

        if (nextLevel == SceneManager.sceneCountInBuildSettings)
        {
            nextLevel = 0;
        }
        
        if(state != State.Dying)
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            SceneManager.LoadScene(currentLevel);
        }
    }

    private void RespondToThrustInput()
    {
        // Can thrust while rotating
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
           audioSource.Stop();
           mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * force * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of the rotation

        // Can't rotate to both sides in the same side
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * Time.deltaTime * rotationSpeed);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void RespondToDebugInput()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            isCollisionOn = !isCollisionOn;
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadLevel();
        }
    }
}
