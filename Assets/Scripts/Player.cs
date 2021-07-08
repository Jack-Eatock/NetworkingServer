using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int id;
    public string username;
    public float moveSpeed = 5f;
    public float jumpspeed = 5f;

    private bool[] inputs;
    private float yVelocity = 0;

    public CharacterController controller;
    public float gravity = -9.91f;

    public Vector2 _inputDirection;
    public Vector3 _moveDirection;

    private void Start() {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpspeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;

        inputs = new bool[5];
    }

    public void FixedUpdate() {

        _inputDirection = Vector2.zero;
        if (inputs[0]) {
            _inputDirection.y += 1;
        }
        if (inputs[1]) {
            _inputDirection.y -= 1;
        }
        if (inputs[2]) {
            _inputDirection.x += 1;
        }
        if (inputs[3]) {
            _inputDirection.x -= 1;
        }


        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection) {


        _moveDirection = -transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded) {
            yVelocity = 0;
            if (inputs[4]) {
                yVelocity = jumpspeed;
            }
        }

        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);



        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, Quaternion _rotation) {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
}
