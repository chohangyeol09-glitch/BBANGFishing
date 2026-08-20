namespace DevLib.ModuleSystem
{
    /// <summary>
    /// ModuleOwner 가 수집하고 초기화하는 모듈의 계약.
    ///
    /// 상속이 자유로운 경우에는 abstract class Module 을 쓰면 되고,
    /// 이미 다른 클래스를 상속했거나 여러 인터페이스를 함께 구현해야 하는 경우
    /// (예: Gun : MonoBehaviour, IWeapon, IReloadable, IModule) 이 인터페이스를 직접 구현한다.
    ///
    /// 다른 모듈을 참조해야 한다면 Initialize 에서는 참조만 확보하고,
    /// 실제 상호작용(구독 등)은 IAfterInitModule.AfterInit 에서 한다.
    /// Initialize 순서는 보장되지 않기 때문이다.
    /// </summary>
    public interface IModule
    {
        void Initialize(ModuleOwner owner);
    }
}
