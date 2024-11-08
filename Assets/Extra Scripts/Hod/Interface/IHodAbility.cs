using System;

public interface IHodAbility
{
    void ActivateAbility();
    bool AbilityPermitted { get; }
    bool CooldownReady { get; }
    
    // Adicionando o evento para sinalizar quando a habilidade é concluída
    event Action OnAbilityCompleted;
}
