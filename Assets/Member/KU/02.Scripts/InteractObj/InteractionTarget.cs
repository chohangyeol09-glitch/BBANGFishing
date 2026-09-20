using UnityEngine;

public class InteractionTarget : MonoBehaviour, IInteractable
{
    [Header("상호작용")]
    [SerializeField]
    private string interactionText = "Interact";

    [SerializeField]
    private bool canInteract = true;

    [Header("실행할 행동들")]
    [SerializeField]
    private InteractionAction[] actions;


    public string GetInteractionText()
    {
        return interactionText;
    }


    public bool CanInteract()
    {
        return canInteract;
    }


    public void Interact()
    {
        if (!canInteract)
            return;


        foreach (InteractionAction action in actions)
        {
            if (action == null)
                continue;

            action.Execute();
        }
    }


    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
}