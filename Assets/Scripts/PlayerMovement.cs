using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AudioClip dashSound;

    [Header("Stamina System")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaRecoveryRate = 1.5f;
    [SerializeField] private float exhaustionCooldown = 2f;
    
    private float currentStamina;
    private bool isExhausted = false;
    private float exhaustionTimer = 0f;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 velocity;
    private float defaultHeight;
    private Vector3 defaultScale;
    private float dashTimer = 0f;
    private bool isCrouching = false;
    private Vector3 dashVelocity = Vector3.zero;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main != null ? Camera.main.transform : transform;
        defaultHeight = controller.height;
        defaultScale = transform.localScale;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (Keyboard.current == null || controller == null) return;

        HandleDashCooldown();
        HandleCrouch();
        HandleExhaustion();
        
        Vector3 moveDirection = CalculateMovement();
        HandleDash(moveDirection);
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleDashCooldown()
    {
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
    }

    private void HandleExhaustion()
    {
        if (isExhausted)
        {
            exhaustionTimer -= Time.deltaTime;
            if (exhaustionTimer <= 0)
            {
                isExhausted = false;
            }
        }
    }

    private Vector3 CalculateMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;

        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            if (cameraTransform != null && cameraTransform != transform) targetAngle += cameraTransform.eulerAngles.y;

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), Time.deltaTime * 10f);

            float currentSpeed = baseSpeed;
            bool isRunning = Keyboard.current.leftShiftKey.isPressed && !isCrouching && !isExhausted;

            if (isRunning)
            {
                currentSpeed *= runMultiplier;
                currentStamina -= Time.deltaTime;
                if (currentStamina <= 0)
                {
                    isExhausted = true;
                    exhaustionTimer = exhaustionCooldown;
                    currentStamina = 0; // Ensure it starts from 0 for recovery
                }
            }
            else
            {
                // Recover stamina when not running
                if (currentStamina < maxStamina)
                {
                    currentStamina += Time.deltaTime * staminaRecoveryRate;
                }
            }

            // Estado agotado: detencion completa
            if (isExhausted)
            {
                velocity.x = 0;
                velocity.z = 0;
                return transform.forward;
            }

            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;

            return moveDirection;
        }
        
        // No input, recover stamina
        if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRecoveryRate;
        }

        velocity.x = 0;
        velocity.z = 0;
        return transform.forward;
    }

    private void HandleCrouch()
    {
        bool crouchPressed = Keyboard.current.leftCtrlKey.isPressed;

        if (crouchPressed && !isCrouching)
        {
            isCrouching = true;
            controller.height = defaultHeight * 0.5f;
            transform.localScale = new Vector3(defaultScale.x, defaultScale.y * 0.5f, defaultScale.z);
        }
        else if (!crouchPressed && isCrouching)
        {
            isCrouching = false;
            controller.height = defaultHeight;
            transform.localScale = defaultScale;
        }
    }

    private void HandleDash(Vector3 moveDirection)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && dashTimer <= 0f && !isCrouching)
        {
            dashVelocity = moveDirection.normalized * dashForce;
            dashTimer = dashCooldown;
            if (dashSound != null && AudioManager.instance != null) AudioManager.instance.PlaySFX(dashSound);
        }

        if (dashVelocity.magnitude > 0.2f)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
    }
}
