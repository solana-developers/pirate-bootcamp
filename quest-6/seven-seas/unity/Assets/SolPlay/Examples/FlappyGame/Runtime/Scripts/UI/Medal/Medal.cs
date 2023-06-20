using System;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.UI.Medal
{
    [System.Serializable]
    public class Medal : IComparable<Medal>
    {
        [field: SerializeField] public Sprite Renderer { get; private set; }
        [field: SerializeField] public int MinPointsRequired { get; private set; }

        public int CompareTo(Medal other)
        {
            return MinPointsRequired > other.MinPointsRequired ? -1 : 1;
        }
    }
}