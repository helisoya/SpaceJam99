using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioPlanet : MonoBehaviour
{
    [SerializeField] private EventReference impact;
    [SerializeField] private EventReference dizzyStars;
    [SerializeField] private EventReference tooHot;
    [SerializeField] private EventReference tooCold;
    [SerializeField] private EventReference cleanPollution;
    [SerializeField] private EventReference fullyPolluted;
    [SerializeField] private EventReference onProjectileSpawn;
    [SerializeField] private EventReference onSunIn;
    [SerializeField] private EventReference onSunOut;
    [SerializeField] private EventReference onFullyCleaned;

    private EventInstance cleaningInstance;

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

    public void OnPitchPlanet(int pitch)
    {
        RuntimeManager.StudioSystem.setParameterByName("PitchPlanet", pitch);
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
        RuntimeManager.PlayOneShot(tooHot);
    }

    public void OnTooCold()
    {
        RuntimeManager.PlayOneShot(tooCold);
    }

    public void OnCleanPollution(bool active)
    {
        if (active)
        {
            cleaningInstance = RuntimeManager.CreateInstance(cleanPollution);
            cleaningInstance.start();
            cleaningInstance.release();
        }
        else
        {
            cleaningInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            cleaningInstance.release();
        }
        
        
    }

    public void OnFullyPolluted()
    {
        RuntimeManager.PlayOneShot(fullyPolluted);
        RuntimeManager.StudioSystem.setParameterByName("Pollution", 10);
    }

    public void OnProjectileSpawn()
    {
        RuntimeManager.PlayOneShot(onProjectileSpawn);
    }

    public void OnSunMovement(bool sun)
    {
        if (sun)
        {
            RuntimeManager.PlayOneShot(onSunIn);
        }

        else
        {
            RuntimeManager.PlayOneShot(onSunOut);
        }
    }

    public void OnFullyCleaned()
    {
        RuntimeManager.PlayOneShot(onFullyCleaned);
    }
}
