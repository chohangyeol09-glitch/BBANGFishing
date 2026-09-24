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


    [Header("첫 번째 활성화 오브젝트")]
    [SerializeField]
    private GameObject firstDelayedObject;

    [SerializeField]
    private float firstActiveDelay = 0.3f;


    [Header("두 번째 활성화 오브젝트")]
    [SerializeField]
    private GameObject secondDelayedObject;

    [SerializeField]
    private float secondActiveDelay = 2f;


    [Header("상점")]
    [SerializeField]
    private ShopSelectManager shopSelectManager;


    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;


    private bool isSelected = false;
    private bool isMoving = false;


    private InteractionTarget interactionTarget;


    private void Awake()
    {
        mainCamera = Camera.main;

        targetObject = transform;


        if (mainCamera != null)
        {
            cameraLook =
                mainCamera.GetComponent<CameraLook>();
        }


        interactionTarget =
            GetComponent<InteractionTarget>();


        if (firstDelayedObject != null)
        {
            firstDelayedObject.SetActive(false);
        }


        if (secondDelayedObject != null)
        {
            secondDelayedObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (!isSelected)
            return;


        if (Keyboard.current == null)
            return;


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscape();
        }
    }


    private void HandleEscape()
    {
        // 상점의 서브 페이지에서 ESC를 눌렀다면
        // 메뉴 화면으로만 돌아가기
        if (shopSelectManager != null)
        {
            bool handled =
                shopSelectManager.TryHandleEscape();


            if (handled)
            {
                return;
            }
        }


        // 메뉴 화면에서 ESC를 눌렀거나
        // ShopSelectManager가 없는 경우
        // 상호작용 자체 종료
        CloseObject();
    }


    public override void Execute()
    {
        if (isSelected || isMoving)
            return;


        SelectObject();
    }


    private void SelectObject()
    {
        if (mainCamera == null ||
            targetObject == null)
            return;


        isSelected = true;


        originalPosition =
            targetObject.position;

        originalRotation =
            targetObject.rotation;

        originalParent =
            targetObject.parent;


        if (interactionTarget != null)
        {
            interactionTarget.SetInteractable(false);
        }


        if (cameraLook != null)
        {
            cameraLook.LockLook();
        }


        if (firstDelayedObject != null)
        {
            firstDelayedObject.SetActive(false);
        }


        if (secondDelayedObject != null)
        {
            secondDelayedObject.SetActive(false);
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


        StartCoroutine(
            ActivateObjectAfterDelay(
                firstDelayedObject,
                firstActiveDelay
            )
        );


        StartCoroutine(
            ActivateObjectAfterDelay(
                secondDelayedObject,
                secondActiveDelay
            )
        );
    }


    private IEnumerator ActivateObjectAfterDelay(
        GameObject target,
        float delay)
    {
        if (target == null)
            yield break;


        yield return new WaitForSeconds(delay);


        if (!isSelected)
            yield break;


        target.SetActive(true);
    }


    private void CloseObject()
    {
        if (!isSelected || isMoving)
            return;


        StopAllCoroutines();


        if (firstDelayedObject != null)
        {
            firstDelayedObject.SetActive(false);
        }


        if (secondDelayedObject != null)
        {
            secondDelayedObject.SetActive(false);
        }


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


        if (interactionTarget != null)
        {
            interactionTarget.SetInteractable(true);
        }


        if (cameraLook != null)
        {
            cameraLook.UnlockLook();
        }
    }
}