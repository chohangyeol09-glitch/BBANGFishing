using UnityEngine;

public class DisableObjectAction : InteractionAction
{
    [SerializeField]
    private GameObject targetObject;


    public override void Execute()
    {
        if (targetObject == null)
            return;

        targetObject.SetActive(false);
    }
}