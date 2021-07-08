using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // SERVER
    public int id;
    public string username;


    // Movement
    public Transform Car;
    public Transform Raypoint;
    public float ForwardAccel = 8f, ReverseAccel = 4f, MaxSpeed = 50f, TurnStrength = 40f, gravityForce = 10f;
    public float Drag = 5f;
    public Transform CentreOfMass;
    public LayerMask whatIsGround;
    public float GroundRayLength = .5f;

    private float ChangeInForwardVelocity = 0;
    private bool isGrounded = true;
    private Rigidbody _carRig;
    private bool[] inputs;
    private Vector2 _inputDirection;


    private void Awake() {
        _carRig = Car.GetComponent<Rigidbody>();
        _carRig.centerOfMass = CentreOfMass.position;
    }

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;

        inputs = new bool[5];
    }

    public void FixedUpdate() {

        _inputDirection = Vector2.zero;
        if (inputs[0]) { _inputDirection.y += 1; }
        if (inputs[1]) { _inputDirection.y -= 1; }
        if (inputs[2]) { _inputDirection.x -= 1; }
        if (inputs[3]) { _inputDirection.x += 1; }

        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection) {

        isGrounded = false;
        if (Physics.Raycast(Raypoint.position, -transform.up, out _, GroundRayLength, whatIsGround)) { isGrounded = true; }

        switch (_inputDirection.y) {
            case (-1): ChangeInForwardVelocity = _inputDirection.y * ReverseAccel; break;
            case (1): ChangeInForwardVelocity = _inputDirection.y * ForwardAccel; break;
            default: break;
        }

        if (isGrounded) {

            var localVelocity = Car.InverseTransformDirection(_carRig.velocity);
            var forwardSpeed = localVelocity.z;
            // if moving
            //Debug.Log(forwardSpeed);
            // forward or back? 

            if (localVelocity.magnitude >= 0.1f) {

                if (forwardSpeed >= 0.01f) { Car.rotation = Quaternion.Euler(Car.rotation.eulerAngles + new Vector3(0f, _inputDirection.x * TurnStrength * 1 * Time.deltaTime, 0f)); }
                else { Car.rotation = Quaternion.Euler(Car.rotation.eulerAngles + new Vector3(0f, _inputDirection.x * TurnStrength * -1 * Time.deltaTime, 0f)); }
            }

            // nullify forces that are not in the forward direction.
           // if (localVelocity.x >= 0.1f) { localVelocity.x = localVelocity.x * 1 / Drag; }
           // Car.transform.TransformDirection(localVelocity);

            _carRig.AddForce(Car.forward * ChangeInForwardVelocity * Time.deltaTime * 100f); ChangeInForwardVelocity = 0;
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, Quaternion _rotation) {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
}
