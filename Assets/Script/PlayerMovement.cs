using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _characterMoveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _gravity = -9.81f;

    private float _verticalVelocity;

    private void Awake()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        // 1. Xử lý trọng lực & giữ nhân vật bám sát mặt sàn (không bị nảy)
        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _gravity * Runner.DeltaTime;
        }

        // 2. Đọc Input di chuyển (WASD / Phím mũi tên)
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y += 1f;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // 3. Xoay mặt mượt mà chỉ quanh trục Y (chống nghiêng ngả tuyệt đối)
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Runner.DeltaTime
            );
        }

        // 4. Gộp di chuyển ngang (X, Z) và trọng lực (Y) vào 1 lệnh Move duy nhất
        Vector3 finalVelocity = moveDirection * _characterMoveSpeed;
        finalVelocity.y = _verticalVelocity;

        _characterController.Move(finalVelocity * Runner.DeltaTime);
    }
}