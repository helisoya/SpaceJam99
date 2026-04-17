using UnityEngine;
using FMODUnity;
public class AudioProjectile : MonoBehaviour
{
    [SerializeField] private EventReference hitProjectile;

    public void OnDestroyed()
    {
        RuntimeManager.PlayOneShot(hitProjectile);
    }
}
