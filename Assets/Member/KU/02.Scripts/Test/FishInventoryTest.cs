using UnityEngine;
using UnityEngine.InputSystem;

public class FishInventoryTest : MonoBehaviour
{
    [SerializeField]
    private FishInventoryManager inventory;

    [SerializeField]
    private FishSO testFish;


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            inventory.AddFish(testFish);
        }
    }
}