using UnityEngine;
using FMODUnity;

public class AudioPlanet : MonoBehaviour
{
    [SerializeField] private EventReference impact;
    [SerializeField] private EventReference dizzyStars;
    [SerializeField] private EventReference tooHot;
    [SerializeField] private EventReference tooCold;
    [SerializeField] private EventReference cleanPollution;
    [SerializeField] private EventReference fullyPolluted;
    [SerializeField] private EventReference onProjectileSpawn;
    

    public void OnRotationLevelChange(int rotationLevel)
    {
        RuntimeManager.StudioSystem.setParameterByName("Rotation", rotationLevel);
        Debug.Log(rotationLevel);
    }

    public void OnHappinessChange(int happiness)
    {
        RuntimeManager.StudioSystem.setParameterByName("Happiness", happiness);
    }

    public void OnTemperatureChange(float temperature)
    {
        RuntimeManager.StudioSystem.setParameterByName("Temperature", temperature);
    }

    public void OnHealthChange(int health)
    {
        RuntimeManager.StudioSystem.setParameterByName("Health", health);
    }

    public void OnImpact()
    {
        RuntimeManager.PlayOneShot(impact);
    }

    public void OnDizzyStars()
    {
        RuntimeManager.PlayOneShot(dizzyStars);
    }

    public void OnTooHot()
    {
        //RuntimeManager.PlayOneShot(tooHot);
    }

    public void OnTooCold()
    {
        RuntimeManager.PlayOneShot(tooCold);
    }

    public void OnCleanPollution()
    {
        RuntimeManager.PlayOneShot(cleanPollution);
    }

    public void OnFullyPolluted()
    {
        //RuntimeManager.PlayOneShot(fullyPolluted);
    }

    public void OnProjectileSpawn()
    {
        //RuntimeManager.PlayOneShot(onProjectileSpawn);
    }
}
