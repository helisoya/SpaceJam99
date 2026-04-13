using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioCheckPouls : MonoBehaviour
{
    public void OnCheck(bool checkPulse)
    {
        if (checkPulse)
        {
            RuntimeManager.StudioSystem.setParameterByName("CheckPulse", 1);
            Debug.Log("Check Pulse");
            //event click to listen to pulse
        }

        else
        {
            RuntimeManager.StudioSystem.setParameterByName("CheckPulse", 0);
            Debug.Log("Check PAS Pulse");
        }
    }
}
