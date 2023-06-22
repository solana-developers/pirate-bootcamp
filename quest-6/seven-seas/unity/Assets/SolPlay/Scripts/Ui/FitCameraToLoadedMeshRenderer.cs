using SolPlay.Utils;
using UnityEngine;

public class FitCameraToLoadedMeshRenderer : MonoBehaviour
{
    public float marginPercentage = 1;
    public GameObject Target;

    private Camera camera;
    
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    
    void Update()
    {
        Bounds bounds = UnityUtils.GetBoundsWithChildren(Target);
        float maxExtent = bounds.extents.magnitude;
        float minDistance = (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2f);
        camera.transform.position = Target.transform.position - Vector3.forward * minDistance;
        camera.nearClipPlane = minDistance - maxExtent;
    }
}