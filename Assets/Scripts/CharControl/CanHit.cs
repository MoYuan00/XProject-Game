using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 可攻击的物体
/// </summary>
[RequireComponent(typeof(Collider))]
public class CanHit : MonoBehaviour
{
    public UnityEvent<Bullet> onHitEvent = new UnityEvent<Bullet>();

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("CanHit OnCollisionEnter");
        if (other.collider.CompareTag("Bullet"))
        {
            onHitEvent?.Invoke(other.gameObject.GetComponent<Bullet>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CanHit OnTriggerEnter");
        if (other.CompareTag("Bullet"))
        {
            onHitEvent?.Invoke(other.gameObject.GetComponent<Bullet>());
        }
    }
}
