using UnityEngine;

/// <summary>
/// Represents a projectile
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    /// <summary>
	/// Initialize the component
	/// </summary>
	/// <param name="direction">The projectile's direction</param>
	/// <param name="speed">The projectile's speed</param>
    public void Init(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.TryGetComponent<Planet>(out Planet planet))
        {
            planet.TakeDamage(gameObject);
        }
    }
}
