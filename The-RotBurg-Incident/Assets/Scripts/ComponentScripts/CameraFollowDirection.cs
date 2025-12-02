using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowDirection : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float flipYRotationTime = 0.5f;

    private Coroutine turnCoroutine;
    private bool isFacingLeft = false;

    private void Update()
    {
        transform.position = playerTransform.position;
    }
    public void CallTurn(bool faceLeft)
    {
        if (isFacingLeft == faceLeft)
        { 
            return;
        }

        isFacingLeft = faceLeft;

        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
        turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRot = transform.localEulerAngles.y;
        float targetRot = isFacingLeft ? 180f : 0f;
        float elapsed = 0f;

        while (elapsed < flipYRotationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flipYRotationTime;

            float yRot = Mathf.Lerp(startRot, targetRot, t);
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, targetRot, 0f);
    }
}
