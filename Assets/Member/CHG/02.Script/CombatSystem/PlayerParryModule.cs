using CHG._02.Script.FishSystem;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHG._02.Script.CombatSystem
{
    /// <summary>
    /// 플레이어 패링 입력. Player(NKT) 프리팹에 컴포넌트로 붙이기만 하면 IModule로 잡힌다.
    /// 돌진 접근 중인 물고기 중 패링 가능한 것(IParryable, ParryModule이 판정)을 찾아 시도한다.
    /// </summary>
    public class PlayerParryModule : MonoBehaviour, IModule
    {
        [SerializeField] private float parryDamage = 0f; //ParryModule이 실제 데미지(최대 체력 비율)로 덮어쓰므로 값은 중요하지 않음
        [SerializeField] private float parryKnockback = 0f;

        private ModuleOwner _owner;

        public void Initialize(ModuleOwner owner) => _owner = owner;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                TryParryNearest();
        }

        private void TryParryNearest()
        {
            Fish[] fishes = FindObjectsByType<Fish>(FindObjectsSortMode.None);
            Fish target = null;
            float bestDist = float.MaxValue;

            foreach (Fish fish in fishes)
            {
                if (!fish.IsParryable) continue;

                float dist = Vector3.Distance(transform.position, fish.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    target = fish;
                }
            }

            if (target == null) return;

            Vector3 dir = (target.transform.position - transform.position).normalized;
            DamageData data = new DamageData(_owner, target.transform.position, -dir, dir, parryDamage, parryKnockback);
            target.TryParry(data);
        }
    }
}
