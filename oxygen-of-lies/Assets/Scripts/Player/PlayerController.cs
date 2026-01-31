using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{   
    [Header("Movement Config")]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _jumpForce = 5f;
    [Header("Ground Check Config")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.1f;

    private PlayerInputHandler _playerInputHandler;
    private Transform _cameraTransform;
    private Rigidbody _rb;

    private void Awake()
    {
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _rb = GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;
        
        _rb.freezeRotation = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _playerInputHandler.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        _playerInputHandler.OnJump -= HandleJump;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = _playerInputHandler.MoveInput;

        // Get camera-relative directions (horizontal only)
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate move direction
        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        // Set velocity directly (preserve Y for gravity)
        _rb.linearVelocity = new Vector3(moveDir.x * _moveSpeed, _rb.linearVelocity.y, moveDir.z * _moveSpeed);
    }

    private void HandleJump()
    {
        if (!IsGrounded()) return;
        
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, _groundCheckRadius, _groundLayer);
    }
}
