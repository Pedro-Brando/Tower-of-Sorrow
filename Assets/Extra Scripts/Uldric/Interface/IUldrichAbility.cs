public interface IUldrichAbility
{
    void ActivateAbility();
    bool AbilityPermitted { get; }
    bool CooldownReady { get; }
}
