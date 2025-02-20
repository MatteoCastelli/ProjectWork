using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UncleAnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;
    MazeGenerator _mazeGenerator;

    int isWalkingHash;
    int isRunningHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    Vector3 _cameraRelativeMovement;
    bool isMovementPressed;
    bool isRunPressed;

    float rotationFactorPerFrame = 10.0f;
    float runMultiplier = 1.5f;

    float gravity = -9.8f;
    float groundedGravity = -.05f;

    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1.0f;
    float maxJumpTime = 1.5f;
    bool isJumping = false;

    int isJumpingHash;
    bool isJumpAnimating = false;


    public string[] animationTriggers = { "Anim1", "Anim2", "Anim3", "Anim4", "Anim5" };
    private bool isAnimating = false;
    private int currentAnimationIndex = 0;


    void Awake()
    {
        _mazeGenerator = FindFirstObjectByType<MazeGenerator>();

        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;


        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        playerInput.CharacterControls.Emote.performed += OnEmotePressed;

        setuupJumpVariables();
    }

    void setuupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void Update()
    {
        if (!isAnimating)
        {
            handleRotation();
            handleAnimation();

            if (isRunPressed)
            {
                appliedMovement.x = currentRunMovement.x;
                appliedMovement.z = currentRunMovement.z;
            }
            else
            {
                appliedMovement.x = currentMovement.x;
                appliedMovement.z = currentMovement.z;
            }

            _cameraRelativeMovement = ConvertToCameraSpace(appliedMovement);
            characterController.Move(_cameraRelativeMovement * Time.deltaTime);

            handleGravity();
            handleJump();
        }
        else
        {
            currentMovement = Vector3.zero;
            currentRunMovement = Vector3.zero;
        }
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {

        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        Debug.Log(isJumpPressed);
    }

    void OnEmotePressed(InputAction.CallbackContext context)
    {
        if (isAnimating)
        {
            StopAnimation();
            isAnimating = false;
        }
        else
        {
            PlayAnimation();
            isAnimating = true;
        }
    }

    void onRun(InputAction.CallbackContext context)
    {
        if (isAnimating) return;
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        if (isAnimating) return;

        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        if (isAnimating) return;

        currentMovementInput = context.ReadValue<Vector2>();

        currentMovement.x = currentMovementInput.x / 1.5f;
        currentMovement.z = currentMovementInput.y / 1.5f;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    void handleAnimation()
    {
        if (isAnimating) return;

        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void handleGravity()
    {

        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20.0f);

        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
            appliedMovement.y = currentMovement.y;
        }

        characterController.Move(new Vector3(0, appliedMovement.y, 0) * Time.deltaTime);
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * .5f;
            appliedMovement.y = initialJumpVelocity * .5f;
        }
        else if (isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    void PlayAnimation()
    {
        if (animationTriggers.Length > 0)
        {
            string chosenTrigger = animationTriggers[currentAnimationIndex];
            animator.SetTrigger(chosenTrigger);
            currentAnimationIndex = (currentAnimationIndex + 1) % animationTriggers.Length;
        }
        else
        {
            Debug.LogWarning("Nessun trigger specificato per le animazioni.");
        }
    }

    void StopAnimation()
    {
        foreach (string trigger in animationTriggers)
        {
            animator.ResetTrigger(trigger);
        }
        animator.Play("Idle");
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("TeleportExit"))
        {
            Vector3 exitPosition = new Vector3(-2, 10, -2);

            characterController.enabled = false;
            transform.position = exitPosition;
            characterController.enabled = true;
        }
    }
}