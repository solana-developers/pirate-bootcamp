using System.Collections.Generic;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Medal
{
    [CreateAssetMenu(menuName = "Data/MedalParameters")]
    public class MedalParameters : ScriptableObject
    {
        [field: SerializeField] public List<Medal> Medals { get; private set; }
        private void OnValidate() => Medals.Sort();
    }
}
