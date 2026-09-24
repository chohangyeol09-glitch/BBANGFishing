using System;
using Unity.Behavior;

namespace CHG._02.Script.BossSystem
{
    [BlackboardEnum]
    public enum BossStateEnum
    {
        Appear, Combat, Groggy, Dead
    }
}