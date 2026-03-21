using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public LineRenderer grappleRope;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintMultiplier = 1.6f;
    public float groundAcceleration = 20f;
    [Range(0f, 1f)]
    public float airControl = 0.3f;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;
    public int maxJumps = 2;
    public float gravity = -20f;
    public float jumpCutMultiplier = 2.5f;

    [Header("Mouse Look")]
    public Vector2 mouseSensitivity = new Vector2(2f, 2f);
    public float verticalLookClamp = 85f;
    public bool invertY = false;

    [Header("Dash (F)")]
    public float dashForce = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.2f;
    public bool canAirDash = true;

    [Header("Grapple (R)")]
    public float grappleMaxDistance = 30f;
    public float grapplePullSpeed = 18f;
    public float grappleLaunchUpward = 3f;
    public float grappleCooldown = 1.5f;
    public LayerMask grappleLayer;
    public int ropeSegments = 20;
    public float ropeWaveAmplitude = 0.5f;
    public float ropeWaveDamping = 8f;

    private const float STANDING_HEIGHT = 2f;

    private CharacterController _cc;

    private Vector3 _velocity;
    private Vector3 _moveVelocity;
    private bool _jumpHeld;
    private int _jumpsRemaining;
    private float _verticalLook;

    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector3 _dashDirection;

    private bool _isGrappling;
    private Vector3 _grapplePoint;
    private float _grappleCooldownTimer;
    private float _ropeWaveTime;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();

        _cc.height = STANDING_HEIGHT;
        _cc.center = Vector3.up * (STANDING_HEIGHT * 0.5f);
        _cc.radius = Mathf.Min(_cc.radius, STANDING_HEIGHT * 0.4f);

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                playerCamera.transform.SetParent(transform);
                playerCamera.transform.localRotation = Quaternion.identity;
            }
            else
            {
                return;
            }
        }

        playerCamera.transform.localPosition = new Vector3(0f, STANDING_HEIGHT - 0.2f, 0f);
        playerCamera.transform.localRotation = Quaternion.identity;

        _jumpsRemaining = maxJumps;

        if (grappleRope != null)
        {
            grappleRope.positionCount = 0;
            grappleRope.enabled = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _cc.enabled = false;

        float floorY = 0f;
        if (Physics.Raycast(transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 200f))
            floorY = hit.point.y;

        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;

        _cc.enabled = true;
    }

    private void Update()
    {
        if (playerCamera == null) return;

        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleDash();
        HandleGrapple();
        ApplyVelocity();
        UpdateRopeVisual();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity.x;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity.y * (invertY ? 1f : -1f);

        transform.Rotate(Vector3.up * mouseX);

        _verticalLook = Mathf.Clamp(_verticalLook + mouseY, -verticalLookClamp, verticalLookClamp);
        playerCamera.transform.localRotation = Quaternion.Euler(_verticalLook, 0f, 0f);
    }

    private void HandleMovement()
    {
        if (_isDashing) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = (transform.right * h + transform.forward * v).normalized;

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = sprinting ? walkSpeed * sprintMultiplier : walkSpeed;

        float accel = _cc.isGrounded ? groundAcceleration : groundAcceleration * airControl;
        _moveVelocity = Vector3.MoveTowards(_moveVelocity, inputDir * speed, accel * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (_cc.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
            _jumpsRemaining = maxJumps;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _jumpsRemaining > 0 && !_isGrappling)
        {
            _velocity.y = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
            _jumpsRemaining--;
            _jumpHeld = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
            _jumpHeld = false;

        float gravScale = (!_jumpHeld && _velocity.y > 0f) ? jumpCutMultiplier : 1f;
        _velocity.y += gravity * gravScale * Time.deltaTime;
    }

    private void HandleDash()
    {
        if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && _dashCooldownTimer <= 0f)
            if (_cc.isGrounded || canAirDash) StartDash();

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f) EndDash();
        }
    }

    private void StartDash()
    {
        _isDashing = true;
        _dashTimer = dashDuration;
        _dashCooldownTimer = dashCooldown;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = (transform.right * h + transform.forward * v).normalized;
        _dashDirection = (input == Vector3.zero) ? transform.forward : input;

        _velocity.y = 0f;
        _moveVelocity = _dashDirection * dashForce;
    }

    private void EndDash()
    {
        _isDashing = false;
        _moveVelocity = _dashDirection * (walkSpeed * 0.5f);
    }

    private void HandleGrapple()
    {
        if (_grappleCooldownTimer > 0f) _grappleCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !_isGrappling && _grappleCooldownTimer <= 0f)
            TryStartGrapple();

        if (Input.GetKeyDown(KeyCode.R) && _isGrappling)
            StopGrapple();

        if (_isGrappling) PullTowardGrapplePoint();
    }

    private void TryStartGrapple()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grappleMaxDistance, grappleLayer))
        {
            _grapplePoint = hit.point;
            _isGrappling = true;
            _ropeWaveTime = 0f;

            Vector3 toPoint = (_grapplePoint - transform.position).normalized;
            _velocity = toPoint * (grapplePullSpeed * 0.5f);
            _velocity.y += grappleLaunchUpward;

            if (grappleRope != null)
            {
                grappleRope.enabled = true;
                grappleRope.positionCount = ropeSegments;
            }
        }
    }

    private void PullTowardGrapplePoint()
    {
        _ropeWaveTime += Time.deltaTime;

        Vector3 toPoint = _grapplePoint - transform.position;
        if (toPoint.magnitude < 1.5f) { StopGrapple(); return; }

        _velocity += toPoint.normalized * grapplePullSpeed * Time.deltaTime * 5f;

        float maxSpeed = grapplePullSpeed * 1.5f;
        if (_velocity.magnitude > maxSpeed)
            _velocity = _velocity.normalized * maxSpeed;

        _moveVelocity = Vector3.zero;
    }

    private void StopGrapple()
    {
        _isGrappling = false;
        _grappleCooldownTimer = grappleCooldown;

        if (grappleRope != null)
        {
            grappleRope.enabled = false;
            grappleRope.positionCount = 0;
        }
    }

    private void UpdateRopeVisual()
    {
        if (!_isGrappling || grappleRope == null || !grappleRope.enabled) return;

        Vector3 start = playerCamera.transform.position;
        Vector3 end = _grapplePoint;
        float wave = ropeWaveAmplitude * Mathf.Exp(-ropeWaveDamping * _ropeWaveTime);
        Vector3 perp = Vector3.Cross((end - start).normalized, Vector3.up).normalized;

        for (int i = 0; i < ropeSegments; i++)
        {
            float t = (float)i / (ropeSegments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            float offset = Mathf.Sin(t * Mathf.PI * 3f - _ropeWaveTime * 10f) * wave * (1f - t);
            grappleRope.SetPosition(i, point + perp * offset);
        }
    }

    private void ApplyVelocity()
    {
        Vector3 finalMove = _isGrappling
            ? _velocity
            : _moveVelocity + Vector3.up * _velocity.y;

        _cc.Move(finalMove * Time.deltaTime);
    }

    public bool IsGrounded => _cc.isGrounded;
    public bool IsGrappling => _isGrappling;
    public bool IsDashing => _isDashing;
    public bool DashReady => _dashCooldownTimer <= 0f;
    public bool GrappleReady => _grappleCooldownTimer <= 0f && !_isGrappling;
    public float GrappleCooldownFraction => Mathf.Clamp01(_grappleCooldownTimer / grappleCooldown);
    public float DashCooldownFraction => Mathf.Clamp01(_dashCooldownTimer / dashCooldown);
}