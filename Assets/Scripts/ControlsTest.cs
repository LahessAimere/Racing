using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.Arm;
using UnityEngine.UI;
using TMPro;

public class ControlsTest : MonoBehaviour
{
    Controls _controls;

    private Vector2 _moveDirection;

    [Header("Inputs settings")]
    //inputs
    private PlayerInput _inputs;
    private Rigidbody _body;

    [Header("Speeds settings")]
    //speed
    public float _accelerationRate = 1;
    public float _maxSpeed;
    private float _currentSpeed;
    private float _currentAcceleration;
    public float _maxRotationSpeed = 1;


    [Header("Cameras settings")]
    //camera
    public CinemachineBrain _brain;
    public CinemachineVirtualCamera _camera1;
    public CinemachineVirtualCamera _camera2;
    public CinemachineVirtualCamera _camera3;
    public CinemachineVirtualCamera _camera4;

    [Header("Audios settings")]
    //Audio
    public AudioSource _audioSource;

    [Header("Texts settings")]
    public TextMeshProUGUI TxtSpeed;

    [Header("wheels settings")]
    public float _wheelRotationFactor = 5;
    public Transform _wheelFrontRight;
    public Transform _wheelFrontLeft;
    public Transform _wheelRearRight;
    public Transform _wheelRearLeft;
    private float _rotationAngle;
    private Vector3 _wheelRotation;

    // Start is called before the first frame update
    private void Start()
    {
        _inputs= GetComponent<PlayerInput>();

        InputAction move = _inputs.actions["Move"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;

        InputAction cameraChange = _inputs.actions["ChangeCamera"];
        cameraChange.performed += OnChangeCamera;

        InputAction cameraRetroLeft = _inputs.actions["RetroLeft"];
        cameraRetroLeft.performed += RetroLeft;

        InputAction cameraRetroRight = _inputs.actions["RetroRight"];
        cameraRetroRight.performed += RetroRight;

        _audioSource = GetComponent<AudioSource>();
        _body = GetComponentInChildren<Rigidbody>();
    }

    private void RetroRight(InputAction.CallbackContext obj)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        Debug.Log("Touch e " + currentCamera.Name);

        if (currentCamera == _camera1 || currentCamera == _camera2 || currentCamera == _camera3)
        {
            _camera4.Priority = 10;
            _camera1.Priority = 0;
            _camera2.Priority = 0;
            _camera3.Priority = 0;
        }

        else
        {
            _camera4.Priority = 0;
            _camera1.Priority = 10;
        }
    }

    private void RetroLeft(InputAction.CallbackContext obj)
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        Debug.Log("Touch a " + currentCamera.Name);

        if (currentCamera == _camera1 || currentCamera == _camera2 || currentCamera == _camera4)
        {
            _camera3.Priority = 10;
            _camera1.Priority = 0;
            _camera2.Priority = 0;
            _camera4.Priority = 0;
        }

        else
        {
            _camera3.Priority = 0;
            _camera1.Priority = 10;
        }
    }

    private void OnChangeCamera(InputAction.CallbackContext obj) 
    {
        var currentCamera = _brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        Debug.Log("Touch Tab " + currentCamera.Name); 

        if (currentCamera == _camera1)
        {
            _camera1.Priority = 0;
            _camera2.Priority = 10;
        }

        else
        {
            _camera2.Priority = 0;
            _camera1.Priority = 10;
        }
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

    void FixedUpdate()
    {
        _body.AddForce(transform.forward * _currentSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        // Récupère les données de mouvement
        float rotationAngle = _moveDirection.x;
        float acceleration = _moveDirection.y;

        // Si on accélère pas (laché)
        if (acceleration == 0)
        {
            _currentAcceleration = Mathf.Lerp(_currentAcceleration, 0, Time.deltaTime);
        }
        else if (acceleration < 0)
        {
            //Si on est en train d'avancer, on freine
            if (_currentAcceleration > 0)
            {
                _currentAcceleration -= Time.deltaTime;
            }
            else // On recule
            {
                _currentAcceleration += acceleration * Time.deltaTime;
            }
        }
        else
        {
            // On accélère progressivement
            _currentAcceleration += acceleration * Time.deltaTime;
        }

        _currentAcceleration = Mathf.Clamp(_currentAcceleration, -1, 1);

        if (_currentAcceleration >= 0)
        {
            _currentSpeed = Mathf.Lerp(0, _maxSpeed, _currentAcceleration);

        }
        else
        {
            _currentSpeed = Mathf.Lerp(0, -_maxSpeed, -_currentAcceleration);
        }

        RotateWheels(rotationAngle);

        // Influence accelerations sur la rotation
        _rotationAngle = rotationAngle * _currentAcceleration * _maxRotationSpeed;

        // Rotation du player.
        // /!\ N'utilise pas la physique pour la rotation, mais simplification ok pour nos besoins
        transform.Rotate(0, _rotationAngle * Time.deltaTime, 0);
        TxtSpeed.text = "Speed : " + (int)acceleration;
    }

    private void RotateWheels(float rotationAngle)
    {
        _wheelRotation.x += _currentSpeed * Time.deltaTime * _wheelRotationFactor;

        if (rotationAngle != 0)
        {
            _wheelRotation.y = Mathf.Clamp(_wheelRotation.y + rotationAngle * Mathf.Sign(_currentSpeed), -30, 30);
        }
        else
        {
            _wheelRotation.y = Mathf.Lerp(_wheelRotation.y, 0, Time.deltaTime);
        }

        _wheelFrontRight.localEulerAngles = _wheelRotation;
        _wheelFrontLeft.localEulerAngles = _wheelRotation;

        Vector3 rearRotation = _wheelRotation;
        rearRotation.y = 0;

        _wheelRearRight.localEulerAngles = rearRotation;
        _wheelRearLeft.localEulerAngles = rearRotation;
    }
}
