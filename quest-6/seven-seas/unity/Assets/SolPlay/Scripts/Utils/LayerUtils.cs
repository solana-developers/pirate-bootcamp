using UnityEngine;

namespace SolPlay.Utils
{
    public class LayerUtils
    {
        public static void SetRenderLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            var children = go.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }
    }
}