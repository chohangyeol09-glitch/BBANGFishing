using System.Collections;
using NKT.Player;
using NKT.Player.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectObjectAction : InteractionAction
{
    [Header("플레이어")]
    [SerializeField]
    private Player player;


    [Header("무기 / 낚싯대")]
    [SerializeField]
    private WeaponModeModule weaponModeModule;


    [Header("이동할 오브젝트")]
    [SerializeField]
    private Transform targetObject;


    [Header("카메라")]
    [SerializeField]
    private Camera mainCamera;


    [Header("이동 설정")]
    [SerializeField]
    private float distanceFromCamera = 1.5f;

    [SerializeField]
    private float moveDuration = 0.3f;


    [Header("최종 위치 보정")]
    [SerializeField]
    private Vector3 positionOffset =
        Vector3.zero;

    [SerializeField]
    private Vector3 rotationOffset =
        Vector3.zero;


    [Header("선택 후 활성화")]
    [SerializeField]
    private GameObject firstObject;

    [SerializeField]
    private GameObject secondObject;


    [SerializeField]
    private float firstObjectDelay = 0.3f;

    [SerializeField]
    private float secondObjectDelay = 2f;


    [Header("상점 UI")]
    [SerializeField]
    private ShopSelectManager shopSelectManager;


    private InteractionTarget interactionTarget;


    private Vector3 originalPosition;

    private Quaternion originalRotation;


    private bool isSelected;

    private bool isMoving;


    private Coroutine moveCoroutine;

    private Coroutine activateCoroutine;



    private void Awake()
    {
        if (targetObject == null)
        {
            targetObject =
                transform;
        }


        if (mainCamera == null)
        {
            mainCamera =
                Camera.main;
        }


        interactionTarget =
            targetObject
                .GetComponent<InteractionTarget>();


        if (interactionTarget == null)
        {
            interactionTarget =
                targetObject
                    .GetComponentInParent<InteractionTarget>();
        }


        originalPosition =
            targetObject.position;


        originalRotation =
            targetObject.rotation;


        if (firstObject != null)
        {
            firstObject.SetActive(false);
        }


        if (secondObject != null)
        {
            secondObject.SetActive(false);
        }
    }



    private void Update()
    {
        if (!isSelected)
            return;


        if (Keyboard.current == null)
            return;


        if (!Keyboard.current
            .escapeKey
            .wasPressedThisFrame)
        {
            return;
        }


        // =========================================
        // 상점 내부 페이지가 열려 있으면
        // 먼저 홈으로 돌아감
        // =========================================

        if (shopSelectManager != null)
        {
            if (shopSelectManager
                .TryHandleEscape())
            {
                return;
            }
        }


        Close();
    }



    public override void Execute()
    {
        if (isSelected)
            return;


        if (isMoving)
            return;


        if (targetObject == null)
            return;


        if (mainCamera == null)
            return;


        isSelected = true;



        // =========================================
        // 현재 들고 있던 장비를 기억하고
        // UI 보는 동안 잠시 숨김
        // =========================================

        if (weaponModeModule != null)
        {
            weaponModeModule
                .HideEquipmentForUI();
        }



        // =========================================
        // 플레이어 입력 / 화면 회전 잠금
        // =========================================

        if (player != null)
        {
            player.LockLook();
        }



        // =========================================
        // 중복 상호작용 방지
        // =========================================

        if (interactionTarget != null)
        {
            interactionTarget
                .SetInteractable(false);
        }



        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );
        }


        moveCoroutine =
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



        // =========================================
        // 카메라 앞 기본 위치
        // =========================================

        Vector3 targetPosition =
            mainCamera.transform.position +
            mainCamera.transform.forward *
            distanceFromCamera;



        // =========================================
        // 위치 Offset
        // =========================================

        targetPosition +=
            mainCamera.transform.right *
            positionOffset.x;


        targetPosition +=
            mainCamera.transform.up *
            positionOffset.y;


        targetPosition +=
            mainCamera.transform.forward *
            positionOffset.z;



        // =========================================
        // 카메라 방향 기준 회전
        // =========================================

        Quaternion baseRotation =
            Quaternion.LookRotation(
                -mainCamera.transform.forward,
                Vector3.up
            );


        Quaternion targetRotation =
            baseRotation *
            Quaternion.Euler(
                rotationOffset
            );



        float time = 0f;


        while (time < moveDuration)
        {
            time +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    time / moveDuration
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


        activateCoroutine =
            StartCoroutine(
                ActivateObjects()
            );
    }



    private IEnumerator ActivateObjects()
    {
        // =========================================
        // 첫 번째 UI
        // =========================================

        if (firstObject != null)
        {
            yield return
                new WaitForSeconds(
                    firstObjectDelay
                );


            firstObject.SetActive(true);
        }



        // =========================================
        // 두 번째 UI
        // =========================================

        float remainingDelay =
            secondObjectDelay -
            firstObjectDelay;


        if (remainingDelay > 0f)
        {
            yield return
                new WaitForSeconds(
                    remainingDelay
                );
        }


        if (secondObject != null)
        {
            secondObject.SetActive(true);
        }
    }



    private void Close()
    {
        if (!isSelected)
            return;


        isSelected = false;



        if (activateCoroutine != null)
        {
            StopCoroutine(
                activateCoroutine
            );


            activateCoroutine =
                null;
        }



        // =========================================
        // UI 닫기
        // =========================================

        if (firstObject != null)
        {
            firstObject.SetActive(false);
        }


        if (secondObject != null)
        {
            secondObject.SetActive(false);
        }



        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );
        }



        // =========================================
        // 오브젝트 원위치
        // =========================================

        moveCoroutine =
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


        float time = 0f;


        while (time < moveDuration)
        {
            time +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    time / moveDuration
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


        isMoving = false;



        // =========================================
        // 다시 상호작용 가능
        // =========================================

        if (interactionTarget != null)
        {
            interactionTarget
                .SetInteractable(true);
        }



        // =========================================
        // UI 열기 전에 들고 있던 장비 복구
        // =========================================

        if (weaponModeModule != null)
        {
            weaponModeModule
                .RestoreEquipmentAfterUI();
        }



        // =========================================
        // 플레이어 조작 복구
        // =========================================

        if (player != null)
        {
            player.UnlockLook();
        }
    }
}