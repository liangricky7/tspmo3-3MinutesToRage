using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    private CharacterController _controller;
    private Camera _camera;
    private float _cameraPitch = 0f;
    private float _verticalVelocity = 0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Gravity & jumping
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f; // Small downward force to keep grounded

            if (Input.GetButtonDown("Jump")) // Space by default
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal and vertical movement
        Vector3 move = (transform.right * x + transform.forward * z) * currentSpeed;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down, clamped to avoid flipping
        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        _camera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }
}