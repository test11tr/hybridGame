using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterControlManager : MonoBehaviour
{
    public static CharacterControlManager Instance;

    [HideInInspector] public T11Joystick joystick;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public Canvas inputCanvas;

    public bool isJoytick;
    public float movementSpeed;
    public float rotationSpeed;

    private Vector3 _moveVector;

    private void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); 
        }

        joystick = GetComponentInChildren<T11Joystick>();
        controller = GetComponentInChildren<CharacterController>();
        rb = GetComponentInChildren<Rigidbody>();
        playerAnimator = GetComponentInChildren<Animator>();
        inputCanvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        if(isJoytick)
            EnableJoystick();
    }

    public void EnableJoystick()
    {
        isJoytick = true;
        inputCanvas.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        _moveVector = Vector3.zero;
        _moveVector.x = joystick.Horizontal * movementSpeed * Time.deltaTime;
        _moveVector.z = joystick.Vertical * movementSpeed * Time.deltaTime;

        if(joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            Vector3 direction = Vector3.RotateTowards(rb.transform.forward, _moveVector, rotationSpeed * Time.deltaTime, 0.0f).normalized;
            rb.transform.rotation = Quaternion.LookRotation(direction);

            playerAnimator.SetBool("isWalking", true);
        }

        else if(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            playerAnimator.SetBool("isWalking", false);
        }

        rb.MovePosition(rb.position + _moveVector);
    }
}
