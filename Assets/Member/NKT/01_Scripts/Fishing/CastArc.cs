using UnityEngine;

namespace NKT.Fishing
{
    //던지는 궤도 공식
    public static class CastArc
    {
        public static Vector3 Evaluate(CastAim aim, float t)
        {
            Vector3 pos = Vector3.Lerp(aim.origin, aim.landPoint, t);
            pos.y += aim.arcHeight * 4f * t * (1f - t);
            return pos;
        }
    }
}
