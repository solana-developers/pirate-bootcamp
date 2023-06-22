using UnityEngine;

namespace SolPlay.Utils
{
    public class UnityUtils
    {
        public static Bounds GetBoundsWithChildren(GameObject gameObject)
        {
            // GetComponentsInChildren() also returns components on gameobject which you call it on
            // you don't need to get component specially on gameObject
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
 
            // If renderers.Length = 0, you'll get OutOfRangeException
            // or null when using Linq's FirstOrDefault() and try to get bounds of null
            Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();
 
            // Or if you like using Linq
            // Bounds bounds = renderers.Length > 0 ? renderers.FirstOrDefault().bounds : new Bounds();
 
            // Start from 1 because we've already encapsulated renderers[0]
            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i].enabled)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }
 
            return bounds;
        }
    }
}