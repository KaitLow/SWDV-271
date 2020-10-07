using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100.0f;
    [SerializeField] float mainThrust = 250f;
    enum State { Alive, Dying, Transcending};
    State state = State.Alive;
    Rigidbody rigidBody;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        { 
            Rotate();
            Thrust();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);           
        }
        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * rotationThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing, someone I like
                break;
            case "Finish":
                //Go to next level
                state = State.Transcending;
                Invoke("LoadNextLevel", 3f);
                break;
            default:
                // die
                state = State.Dying;
                Invoke("LoadFirstLevel", 3f);
                break;
        }

    }

    private void LoadFirstLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }
}
