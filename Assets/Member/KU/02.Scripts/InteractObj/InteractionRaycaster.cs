using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionRaycaster : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private float interactionDistance = 3f;

    [SerializeField]
    private LayerMask interactionLayerMask = ~0;


    [Header("Input")]
    [SerializeField]
    private InputActionReference interactAction;


    [Header("UI")]
    [SerializeField]
    private GameObject interactionUI;

    [SerializeField]
    private TMP_Text interactionText;


    private IInteractable currentInteractable;


    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        HideUI();
    }


    private void OnEnable()
    {
        interactAction?.action.Enable();
    }


    private void OnDisable()
    {
        interactAction?.action.Disable();

        currentInteractable = null;

        HideUI();
    }


    private void Update()
    {
        DetectInteractable();
        CheckInteractionInput();
    }


    private void DetectInteractable()
    {
        if (targetCamera == null)
        {
            ClearInteractable();
            return;
        }


        Ray ray = new Ray(
            targetCamera.transform.position,
            targetCamera.transform.forward
        );


        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactionDistance,
                interactionLayerMask,
                QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable =
                FindInteractable(hit.collider);


            if (interactable != null &&
                interactable.CanInteract())
            {
                SetInteractable(interactable);
                return;
            }
        }


        ClearInteractable();
    }


    private IInteractable FindInteractable(Collider collider)
    {
        Transform current = collider.transform;


        while (current != null)
        {
            MonoBehaviour[] behaviours =
                current.GetComponents<MonoBehaviour>();


            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInteractable interactable)
                {
                    return interactable;
                }
            }


            current = current.parent;
        }


        return null;
    }


    private void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;


        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }


        if (interactionText != null)
        {
            interactionText.text =
                $"{interactable.GetInteractionText()}";
        }
    }


    private void ClearInteractable()
    {
        currentInteractable = null;

        HideUI();
    }


    private void CheckInteractionInput()
    {
        if (currentInteractable == null)
            return;


        if (interactAction == null)
            return;


        if (interactAction.action.WasPressedThisFrame())
        {
            currentInteractable.Interact();
        }
    }


    private void HideUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (targetCamera == null)
            return;


        Gizmos.DrawRay(
            targetCamera.transform.position,
            targetCamera.transform.forward * interactionDistance
        );
    }

#endif
}