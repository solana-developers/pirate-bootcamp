namespace SolPlay.FlappyGame.Runtime.Scripts.Level.Pipes
{
    [System.Serializable]
    public class PipeGroup
    {
        public Pipe topPipe;
        public Pipe bottomPipe;
        public float XPosition => bottomPipe.transform.position.x;
    }
}
