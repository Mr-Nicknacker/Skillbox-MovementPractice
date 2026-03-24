using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody _playerRigidBody;
    private Collider _playerCollider;
    private float _playerHeight;
    private float _playerToGroundDistance, _mouseToGroundDistance;
    private Camera _mainCamera;
    private Vector2 _movementDirection;
    private Vector3 _mousePosition;
    private Vector3 _lookDirection;
    private Vector3 _mouseWorldPosition;
    private Vector3 _movementVector;
    private bool isGrounded = true;


    private void Start()
    {
        _mainCamera = Camera.main;
        _playerRigidBody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<Collider>();
        _playerHeight = _playerCollider.bounds.size.y;
        _playerToGroundDistance = _playerHeight / 2f + 0.5f;

        Debug.Log($"collider size: {_playerCollider.bounds.size.y}");
    }
    void Update()
    {
        transform.Translate(_movementVector * _moveSpeed * Time.deltaTime);
    }
    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            _movementDirection = context.ReadValue<Vector2>();
            _movementVector = new Vector3(_movementDirection.x, 0f, _movementDirection.y);
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(transform.position, Vector3.down, _playerToGroundDistance, _groundLayer))
        {
            isGrounded = true;
        }
        if (isGrounded && context.performed)
        {
            _playerRigidBody.AddForce(Vector3.up * calculateJumpingVelocity(_jumpHeight), ForceMode.Impulse); //почему-то подлетает не на 2
            isGrounded = false;
        }
    }
    private float calculateJumpingVelocity(float jumpHeight)
    {
        if (jumpHeight <= 0)
        {
            return 0;
        }
        return Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
    }

    public void RotatePlayer(InputAction.CallbackContext context)
    {
        Quaternion newRotation;
        _mousePosition = context.ReadValue<Vector2>();
        _mouseWorldPosition = GetMouseWorldPosition(_mousePosition, _mainCamera);
        _mouseToGroundDistance = _mouseWorldPosition.y+100f;

        Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
        float angleDiff = Quaternion.Angle(_mainCamera.transform.rotation, targetRotation);

        if (_mainCamera.orthographic && angleDiff==0f)
        {
            _lookDirection = (_mouseWorldPosition - _playerModel.position).normalized;
            _lookDirection.y = 0f;
            newRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
            _playerModel.rotation = newRotation;
        }
        else
        {
            Ray ray = _mainCamera.ScreenPointToRay(_mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, _mouseToGroundDistance, _groundLayer);
            _lookDirection = (hit.point - _playerModel.position).normalized;
            _lookDirection.y = 0f;
            newRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
            _playerModel.rotation = newRotation;
        }
    }
    private Vector3 GetMouseWorldPosition(Vector3 position, Camera camera)
    {
        Vector3 _cursorWorldPosition = camera.ScreenToWorldPoint(position);
        return _cursorWorldPosition;
    }
}
