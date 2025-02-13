﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    // Start is called before the first frame update

    [SerializeField] float rcsThrust = 5f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip successSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    [SerializeField] int currentLevelIndex = 0;
    [SerializeField] int nextLevelIndex = 0;

    [SerializeField] KeyCode nextLevelHotKey = KeyCode.Tab;
    [SerializeField] KeyCode unbreakableHotKey = KeyCode.CapsLock;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool breakable = true;

    enum State {
        Alive, Dying, Transcending
    }

    State state = State.Alive;

    void Start() {
        ResetLevelIndex();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void ResetLevelIndex() {
        if (currentLevelIndex == 0) {
            currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        }
        if ((nextLevelIndex == 0) && (currentLevelIndex != SceneManager.sceneCountInBuildSettings - 1)) {
            nextLevelIndex = currentLevelIndex + 1;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(unbreakableHotKey)) {
            breakable = !breakable;
        }
        if (Input.GetKeyDown(nextLevelHotKey)) {
            LoadNextLevel();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) {
            return; // ignore further collision
        }
        switch (collision.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                LevelUp();
                break;
            case "Fuel":
                //
                break;
            case "Ammo":
                //
                break;
            default:
                if (breakable) {
                    Die();
                }
                break;
        }
    }

    private void Die() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("Reborn", levelLoadDelay);
    }

    private void LevelUp() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void Reborn() {
        SceneManager.LoadScene(currentLevelIndex);
        audioSource.Stop();
    }

    private void LoadNextLevel() {
        SceneManager.LoadScene(nextLevelIndex);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngineSound);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
        rigidBody.angularVelocity = Vector3.zero;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}
