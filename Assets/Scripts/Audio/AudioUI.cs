using UnityEngine;
using FMODUnity;

public class AudioUI : MonoBehaviour
{
    [SerializeField] private EventReference centralClick;
    [SerializeField] private EventReference crankUp;
    [SerializeField] private EventReference crankDown;
    [SerializeField] private EventReference tutoIn;
    [SerializeField] private EventReference tutoOut;
    [SerializeField] private EventReference giveName;
    [SerializeField] private EventReference startGame;

    public void OnCentralClick()
    {
        RuntimeManager.PlayOneShot(centralClick);
    }

    public void OnCrankUp()
    {
        RuntimeManager.PlayOneShot(crankUp);
    }

    public void OnCrankDown()
    {
        RuntimeManager.PlayOneShot(crankDown);
    }

    public void OnTutorial(bool tutorialIn)
    {
        if (tutorialIn)
        {
            RuntimeManager.PlayOneShot(tutoIn);
        }
        else
        {
            RuntimeManager.PlayOneShot(tutoOut);
        }
    }

    public void OnGiveName()
    {
        RuntimeManager.PlayOneShot(giveName);
    }

    public void OnStartGame()
    {
        RuntimeManager.PlayOneShot(startGame);
    }

}
