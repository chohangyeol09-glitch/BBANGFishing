using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObjectAction : InteractionAction
{
    private Camera mainCamera;
    private CameraLook cameraLook;

    private Transform targetObject;


    [Header("카메라 앞 위치")]
    [SerializeField]
    private float distanceFromCamera = 1f;

    [SerializeField]
    private Vector3 positionOffset;


    [Header("회전")]
    [SerializeField]
    private Vector3 rotationOffset;


    [Header("이동 연출")]
    [SerializeField]
    private float moveDuration = 0.3f;


    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;


    private bool isSelected = false;
    private bool isMoving = false;


    private InteractionTarget interactionTarget;


    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        targetObject = gameObject.transform;

        cameraLook = mainCamera.GetComponent<CameraLook>();


        interactionTarget =
            GetComponent<InteractionTarget>();
    }


    private void Update()
    {
        if (!isSelected)
            return;


        if (Keyboard.current == null)
            return;


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseObject();
        }
    }


    public override void Execute()
    {
        if (isSelected || isMoving)
            return;

        SelectObject();
    }


    private void SelectObject()
    {
        if (mainCamera == null || targetObject == null)
            return;


        isSelected = true;


        // 원래 위치 저장
        originalPosition = targetObject.position;
        originalRotation = targetObject.rotation;
        originalParent = targetObject.parent;


        // 상호작용 중 다시 E가 뜨지 않도록
        if (interactionTarget != null)
        {
            interactionTarget.SetInteractable(false);
        }


        // 카메라 움직임 정지
        if (cameraLook != null)
        {
            cameraLook.LockLook();
        }


        StopAllCoroutines();

        StartCoroutine(
            MoveToCamera()
        );
    }


    private IEnumerator MoveToCamera()
    {
        isMoving = true;


        Vector3 startPosition =
            targetObject.position;

        Quaternion startRotation =
            targetObject.rotation;


        Vector3 targetPosition =
            mainCamera.transform.position
            + mainCamera.transform.forward * distanceFromCamera
            + mainCamera.transform.right * positionOffset.x
            + mainCamera.transform.up * positionOffset.y
            + mainCamera.transform.forward * positionOffset.z;


        Quaternion targetRotation =
            mainCamera.transform.rotation *
            Quaternion.Euler(rotationOffset);


        float elapsedTime = 0f;


        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / moveDuration
                );


            // 부드러운 이동
            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );


            targetObject.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );


            targetObject.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );


            yield return null;
        }


        targetObject.position =
            targetPosition;

        targetObject.rotation =
            targetRotation;


        isMoving = false;
    }


    private void CloseObject()
    {
        if (!isSelected || isMoving)
            return;


        StopAllCoroutines();

        StartCoroutine(
            ReturnObject()
        );
    }


    private IEnumerator ReturnObject()
    {
        isMoving = true;


        Vector3 startPosition =
            targetObject.position;

        Quaternion startRotation =
            targetObject.rotation;


        float elapsedTime = 0f;


        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / moveDuration
                );


            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );


            targetObject.position =
                Vector3.Lerp(
                    startPosition,
                    originalPosition,
                    t
                );


            targetObject.rotation =
                Quaternion.Slerp(
                    startRotation,
                    originalRotation,
                    t
                );


            yield return null;
        }


        targetObject.position =
            originalPosition;

        targetObject.rotation =
            originalRotation;

        targetObject.SetParent(
            originalParent
        );


        isSelected = false;
        isMoving = false;


        // 다시 상호작용 가능
        if (interactionTarget != null)
        {
            interactionTarget.SetInteractable(true);
        }


        // 카메라 회전 다시 활성화
        if (cameraLook != null)
        {
            cameraLook.UnlockLook();
        }
    }
}