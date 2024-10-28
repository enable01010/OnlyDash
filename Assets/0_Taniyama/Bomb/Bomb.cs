using JetBrains.Annotations;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    const float PREFAB_DESTROY_TIME = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<I_BombHit>(out var hit)) return;

        hit.BombHit();
        GameObject instance = Instantiate(LibResourceLoader._bombFxPref);
        instance.transform.position = transform.position;
        Destroy(instance, PREFAB_DESTROY_TIME);
        Destroy(gameObject);
    }
}

public interface I_BombHit
{
    public void BombHit();
}
