using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterControlManager : MonoBehaviour
{
    public T11Joystick joystick;
    public CharacterController controller;
    public Animator playerAnimator;
    public Canvas inputCanvas;
    public bool isJoytick;

    public float movementSpeed;
    public float rotationSpeed;

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

    private void Update()
    {
        if (isJoytick)
        {
            var movementDirection = new Vector3(joystick.Direction.x, 0, joystick.Direction.y).normalized;
            controller.SimpleMove(movementDirection * movementSpeed);

            if(movementDirection.sqrMagnitude <= 0)
            {
                playerAnimator.SetBool("isWalking", false);
                return;
            }

            playerAnimator.SetBool("isWalking", true);
            var targetDirection = Vector3.RotateTowards(controller.transform.forward, movementDirection, rotationSpeed * Time.deltaTime, 0.0f);
            controller.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}
