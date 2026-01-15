using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Player Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;

    [Header("Cutscene Cameras")]
    [SerializeField] private CinemachineVirtualCamera[] cutsceneCameras;

    [Header("Player jump/fall lerping")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;
    private Coroutine shakeCoroutine;

    private CinemachineVirtualCamera activeCamera;
    private CinemachineFramingTransposer framingTransposer;
    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private float normYPanAmount;
    private Vector2 startingTrackedObjectOffset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        activeCamera = playerCamera;
        CacheCameraComponents(playerCamera);

        normYPanAmount = framingTransposer.m_YDamping;
        startingTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;

        ResetAllCameraPriorities();
        playerCamera.Priority = 20;
    }

    private void CacheCameraComponents(CinemachineVirtualCamera cam)
    {
        if (cam == null)
        {
            return;
        }
        activeCamera = cam;
        framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        perlinNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void ResetAllCameraPriorities()
    {
        playerCamera.Priority = 0;

        foreach (var cam in cutsceneCameras)
        {
            if (cam != null)
            {
                cam.Priority = 0;
            }
        }
    }

    public void SwitchToCutsceneCamera(int cutsceneIndex)
    {
        if (cutsceneIndex < 0 || cutsceneIndex >= cutsceneCameras.Length)
        {
            return;
        }

        ResetAllCameraPriorities();

        CinemachineVirtualCamera cutsceneCam = cutsceneCameras[cutsceneIndex];
        cutsceneCam.Priority = 20;

        CacheCameraComponents(cutsceneCam);
    }

    public void ReturnToPlayerCamera()
    {
        ResetAllCameraPriorities();

        playerCamera.Priority = 20;
        CacheCameraComponents(playerCamera);

        framingTransposer.m_YDamping = normYPanAmount;
        framingTransposer.m_TrackedObjectOffset = startingTrackedObjectOffset;
    }

    public void LerpYDamping(bool isPlayerFalling)
    {
        if (lerpYPanCoroutine != null)
        {
            StopCoroutine(lerpYPanCoroutine);
        }

        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = isPlayerFalling ? fallPanAmount : normYPanAmount;

        LerpedFromPlayerFalling = isPlayerFalling;

        float elapsedTime = 0f;
        while (elapsedTime < fallPanTime)
        {
            elapsedTime += Time.deltaTime;
            framingTransposer.m_YDamping = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallPanTime);
            yield return null;
        }

        IsLerpingYDamping = false;
    }

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        if (panCameraCoroutine != null)
        {
            StopCoroutine(panCameraCoroutine);
        }

        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 startingPos;
        Vector2 endPos;

        if (!panToStartingPos)
        {
            startingPos = startingTrackedObjectOffset;

            endPos = panDirection switch
            {
                PanDirection.Up => Vector2.up,
                PanDirection.Down => Vector2.down,
                PanDirection.Left => Vector2.left,
                PanDirection.Right => Vector2.right,
                _ => Vector2.zero
            };

            endPos = startingPos + (endPos * panDistance);
        }
        else
        {
            startingPos = framingTransposer.m_TrackedObjectOffset;
            endPos = startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            framingTransposer.m_TrackedObjectOffset = Vector2.Lerp(startingPos, endPos, elapsedTime / panTime);
            yield return null;
        }
    }

    public void ScreenShake(float intensity, float duration)
    {
        if (perlinNoise == null)
        {
            Debug.LogWarning("No CinemachineBasicMultiChannelPerlin found on active camera.");
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        perlinNoise.m_AmplitudeGain = intensity;
        perlinNoise.m_FrequencyGain = 2f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fade = 1f - (elapsed / duration);
            perlinNoise.m_AmplitudeGain = intensity * fade;
            yield return null;
        }

        perlinNoise.m_AmplitudeGain = 0f;
    }
}
