using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Player
{
    [CreateAssetMenu(menuName = "Data/PlayerParameters")]
    public class PlayerParameters : ScriptableObject
    {
        [field: SerializeField] public float FlapSpeed { get; private set; } = 2.5f;
        [field: SerializeField] public float ForwardSpeed { get; private set; }
        [field: SerializeField] public float ForwardSpeedIncrease { get; private set; }
        [field: SerializeField, Min(0)] public float Gravity { get; private set; }

        [field: SerializeField, Range(-180, 0)] public float FallingRotationAngle { get; private set; }  
        [field: SerializeField] public float FallingRotationSpeed { get; private set; }
        [field: SerializeField, Range(0, 180)] public float FlapRotation { get; private set; }
    }
}
