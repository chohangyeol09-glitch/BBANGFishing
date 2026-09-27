using CHG._02.Script.FishSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishInventoryTest : MonoBehaviour
{
    [SerializeField]
    private FishInventoryManager inventory;

    [SerializeField]
    private FishDataSO testFish;


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