using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Physics Push")]
    public float pushForce = 8f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    [HideInInspector] public bool freeze = false;

    private CharacterController _controller;
    private Camera _camera;
    private float _cameraPitch = 0f;
    private float _verticalVelocity = 0f;

    private Vector3? _grappleTarget = null;
    private float _grappleArcHeight = 0f;
    private Vector3 _grappleStartPos;
    private float _grappleArcTime = 0f;
    private float _grappleArcDuration = 0f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (freeze) return;

        if (_grappleTarget.HasValue)
            HandleGrappleArc();
        else
            HandleMovement();

        HandleMouseLook();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = (transform.right * x + transform.forward * z) * currentSpeed;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    public void JumpToPosition(Vector3 targetPosition, float arcHeight)
    {
        _grappleTarget = targetPosition;
        _grappleStartPos = transform.position;
        _grappleArcHeight = arcHeight;
        _grappleArcTime = 0f;

        float distance = Vector3.Distance(transform.position, targetPosition);
        _grappleArcDuration = Mathf.Clamp(distance / 25f, 0.4f, 1.5f);

        _verticalVelocity = 0f;
    }

    void HandleGrappleArc()
    {
        _grappleArcTime += Time.deltaTime;
        float t = Mathf.Clamp01(_grappleArcTime / _grappleArcDuration);

        Vector3 currentPos = transform.position;
        Vector3 targetPos = _grappleTarget.Value;

        Vector3 nextPos = Vector3.Lerp(_grappleStartPos, targetPos, t);
        nextPos.y += _grappleArcHeight * Mathf.Sin(Mathf.PI * t);

        Vector3 delta = nextPos - currentPos;
        _controller.Move(delta);

        if (t >= 1f)
        {
            _grappleTarget = null;
            _verticalVelocity = 0f;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        _camera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        rb.velocity = pushDir * pushForce;
    }
}