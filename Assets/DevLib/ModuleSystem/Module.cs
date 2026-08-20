using UnityEngine;

namespace DevLib.ModuleSystem
{
    /// <summary>
    /// IModule 의 기본 구현. _owner 보관까지 대신해 준다.
    /// 다른 클래스를 상속해야 하거나 인터페이스 조합이 필요하면 IModule 을 직접 구현한다.
    /// </summary>
    public abstract class Module : MonoBehaviour, IModule
    {
        protected ModuleOwner _owner;

        public virtual void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }
    }
}
