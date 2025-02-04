using System.Collections.Generic;
using UnityEngine.Rendering.VirtualTexturing;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of toRegister to add
        var toRegister = new List<ISystem>();

        // Add your toRegister here

        toRegister.Add(new InitializationSystem());
        toRegister.Add(new MovementSystem());
        toRegister.Add(new WallCollisionSystem());
        toRegister.Add(new CollisionSystem());
        toRegister.Add(new ExplosionSystem());
        toRegister.Add(new ProtectionSystem());
        toRegister.Add(new InputSystem());
        toRegister.Add(new TimeAccelerationSystem());
        toRegister.Add(new RenderSystem());
        toRegister.Add(StateHistorySystem.Instance); // Singleton instance


        return toRegister;
    }
}