﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100.0f;
    [SerializeField] float mainThrust = 250f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] float levelLoadDelay = 3f;

    bool collisionDisabled = false;


    enum State { Alive, Dying, Transcending };
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
        if (state == State.Alive)
        {
            RespondToRotateInput();
            RespondToThrustInput();
        }
        
        //if(Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;
        }
    }

    private void RespondToRotateInput()
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

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        ApplyThrust(thrustThisFrame);
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            if (!mainEngineParticles.isPlaying)
            {
                mainEngineParticles.Play();
            }
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDisabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing, someone I like
                break;
            case "Finish":
                // Go to next level
                StartSuccessSequence();
                break;
            default:
                // die
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();

        state = State.Dying;
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        state = State.Transcending;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        state = State.Alive;
        int currentScenceIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentScenceIndex + 1;
        print("SceneCount " + SceneManager.sceneCountInBuildSettings);
        print("Current Scene Index " + currentScenceIndex);
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
          nextSceneIndex = 0;
        }
        print("Next Scene Index " + nextSceneIndex);
        SceneManager.LoadScene(nextSceneIndex);
    }
}
