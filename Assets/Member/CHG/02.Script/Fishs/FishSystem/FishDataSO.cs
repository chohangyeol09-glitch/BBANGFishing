using UnityEngine;

namespace Member.CHG._02.Script.FIsh.FishSystem
{
    [CreateAssetMenu(fileName = "Fish data", menuName = "CHG/Fish/FishData", order = 0)]
    public class FishDataSO : ScriptableObject
    {
        public float Health;
        public float Weight;

        public int Price;
        
        

    }
}