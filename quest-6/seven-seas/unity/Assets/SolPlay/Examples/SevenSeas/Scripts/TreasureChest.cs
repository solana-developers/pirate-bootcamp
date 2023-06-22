using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GameObject CollectFXPrefab;

    public void Init(Vector2 startPosition)
    {
        transform.position = new Vector3(10 * startPosition.x + 5f, 1.4f, (10 * startPosition.y) - 5f);
    }

    public void Collect()
    {
        if (CollectFXPrefab != null)
        {
            var collectFx = Instantiate(CollectFXPrefab);
            collectFx.transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}