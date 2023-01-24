using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsTest : MonoBehaviour
{
    Controls _controls;

    private Vector2 _moveDirection;

    public InputAction _inputAction;

    private PlayerInput _inputs;

    // Start is called before the first frame update
    void Start()
    {
        _inputs= GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];
        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
       _moveDirection= Vector2.zero;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"Move Performed : {context.ReadValue<Vector2>()}");
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"Move started : {context.ReadValue<Vector2>()}");
        _moveDirection = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3)_moveDirection*Time.deltaTime;
    }
}
