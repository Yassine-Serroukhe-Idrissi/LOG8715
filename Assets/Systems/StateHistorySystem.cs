using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Records a short (3-second) history of each entity’s state to allow rewinding.
/// Implemented as a singleton for easy access from other systems (such as InputSystem).
/// </summary>
public class StateHistorySystem : ISystem
{
    public string Name => "StateHistorySystem";

    private static StateHistorySystem _instance;
    public static StateHistorySystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = new StateHistorySystem();
            return _instance;
        }
    }

    // Duration (in seconds) for which history is maintained.
    private readonly float _historyDuration = 3f;

    // Stores a history queue for each entity.
    private readonly Dictionary<uint, Queue<HistoryRecord>> _history = new Dictionary<uint, Queue<HistoryRecord>>();

    // Represents a snapshot of an entity's state.
    private class HistoryRecord
    {
        public float TimeStamp;
        public Vector2 Position;
        public Vector2 Velocity;
        public int Size;
        public ProtectionState ProtState;

        public HistoryRecord(float time, Vector2 pos, Vector2 vel, int size, ProtectionState prot)
        {
            TimeStamp = time;
            Position = pos;
            Velocity = vel;
            Size = size;
            ProtState = prot;
        }
    }

    public void UpdateSystem()
    {
        float currentTime = Time.time;
        // Record state for every entity.
        foreach (var entity in EntityManager.GetAllEntities())
        {
            var posComp = entity.GetComponent<PositionComponent>();
            var velComp = entity.GetComponent<VelocityComponent>();
            var sizeComp = entity.GetComponent<SizeComponent>();
            var protComp = entity.GetComponent<ProtectionComponent>();

            if (posComp == null || velComp == null || sizeComp == null)
                continue;

            var record = new HistoryRecord(currentTime, posComp.Position, velComp.Velocity, sizeComp.Size,
                                           protComp != null ? protComp.State : ProtectionState.None);

            if (!_history.ContainsKey(entity.Id))
                _history[entity.Id] = new Queue<HistoryRecord>();

            _history[entity.Id].Enqueue(record);

            // Remove records older than the history duration.
            while (_history[entity.Id].Count > 0 && currentTime - _history[entity.Id].Peek().TimeStamp > _historyDuration)
                _history[entity.Id].Dequeue();
        }
    }

    /// <summary>
    /// Rewinds the state of all entities to the state from "seconds" ago.
    /// </summary>
    public void Rewind(float seconds)
    {
        float targetTime = Time.time - seconds;
        foreach (var kvp in _history)
        {
            uint entityId = kvp.Key;
            HistoryRecord chosen = null;
            foreach (var record in kvp.Value)
            {
                if (record.TimeStamp <= targetTime)
                    chosen = record;
                else
                    break;
            }
            if (chosen != null)
            {
                Entity entity = EntityManager.GetEntity(entityId);
                if (entity != null)
                {
                    var posComp = entity.GetComponent<PositionComponent>();
                    var velComp = entity.GetComponent<VelocityComponent>();
                    var sizeComp = entity.GetComponent<SizeComponent>();
                    var protComp = entity.GetComponent<ProtectionComponent>();

                    posComp.Position = chosen.Position;
                    velComp.Velocity = chosen.Velocity;
                    sizeComp.Size = chosen.Size;
                    if (protComp != null)
                        protComp.State = chosen.ProtState;

                    ECSController.Instance.UpdateShapePosition(entity.Id, posComp.Position);
                    ECSController.Instance.UpdateShapeSize(entity.Id, sizeComp.Size);
                }
            }
        }
    }
}
