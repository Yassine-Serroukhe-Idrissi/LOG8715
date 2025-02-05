using UnityEngine;

//Etat du cercle : protege, explosif
public enum State
{
    None,
    Protected,
    CoolDown
}
public struct StateComponent: IComponent
{
    public State state;
    public float protectionStartTime; // Temps du d�but de la protection
    public float cooldownStartTime;
}
