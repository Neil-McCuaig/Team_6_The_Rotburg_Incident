using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Arm Settings")]
    public Transform arm; 
    public bool flipArmLeft = true; 
    public SpriteRenderer spriteRender;

    [Header("Input Mapping Controls")]
    public InputActionAsset inputActions;
    private InputAction aimAction;
    [SerializeField] private InputAction flashAction;

    private Vector2 aimInput;
    Vector2 lastAimDirection = Vector2.right; 
    float lastAngle = 0f;

    [Header("References")]
    public GameObject stunEffect;
    public Transform cameraFlash;
    public Transform flashLeft;
    public Transform flashRight;
    private bool canFlash = true;
    public SpriteRenderer effectRender;

    private void Awake()
    {
        var playerActions = inputActions.FindActionMap("BaseGameplay");
        aimAction = playerActions.FindAction("AimDirection");
    }

    private void OnEnable()
    {
        aimAction.Enable();
        flashAction.Enable();
    }

    // Disable the input actions
    private void OnDisable()
    {
        aimAction.Disable();
        flashAction.Disable();
    }

    void Update()
    {
        AimingDirection();

        if (flashAction.WasPressedThisFrame() && canFlash)
        {
            ActivateFlash();
        }
    }

    void AimingDirection()
    {        
        aimInput = aimAction.ReadValue<Vector2>();

        if (aimInput.sqrMagnitude > 0.01f)
        {
            lastAimDirection = aimInput.normalized;
            lastAngle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        }

        arm.rotation = Quaternion.Euler(0f, 0f, lastAngle);

        if (flipArmLeft)
        {
            if (lastAngle > 90 || lastAngle < -90)
            {
                spriteRender.flipY = true;
                cameraFlash.position = flashLeft.position;
            }
            else
            {
                spriteRender.flipY = false;
                cameraFlash.position = flashRight.position;
            }
        }
    }

    void ActivateFlash()
    {
        canFlash = false;
        stunEffect.SetActive(true);
        Color originalColor = effectRender.color;
        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        float elapsed = 0f;
        Color originalColor = effectRender.color;

        while (elapsed < 1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        stunEffect.SetActive(false);
        canFlash = true;
    }
}
