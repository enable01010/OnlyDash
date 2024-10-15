using JetBrains.Annotations;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<I_BombHit>(out var hit)) return;

        hit.Hit();

        Destroy(gameObject);
    }
}

public interface I_BombHit
{
    public void Hit();
}
