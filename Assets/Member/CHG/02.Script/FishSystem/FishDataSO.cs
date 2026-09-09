using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    [CreateAssetMenu(fileName = "Fish data", menuName = "CHG/Fish/Fish data", order = 0)]
    public class FishDataSO : ScriptableObject
    {
        public Grade Grade;
        public float Health;
        public float Weight;
        public float JumpPower;
        

        public int Price;
        
        

    }
}