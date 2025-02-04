//Enumeration des etas de protection du cercle
public enum ProtectionState
{
    None,
    Protected,
    Cooldown,
}

public class ProtectionComponent : IComponent
{
    public ProtectionState State;
    public float ProtectionTimer;
    public float CooldownTimer;
    public int CollisionCount;

    //Constructeur
    public ProtectionComponent()
    {
        State = ProtectionState.None;
        ProtectionTimer = 0f;
        CooldownTimer = 0f;
        CollisionCount = 0;
    }
}