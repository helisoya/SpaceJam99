using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioUI : MonoBehaviour
{
    [SerializeField] private EventReference centralClick;
    [SerializeField] private EventReference crankUp;
    [SerializeField] private EventReference crankDown;
    [SerializeField] private EventReference tutoIn;
    [SerializeField] private EventReference tutoOut;
    [SerializeField] private EventReference giveName;
    [SerializeField] private EventReference startGame;
    [SerializeField] private EventReference gameMusic;
    [SerializeField] private EventReference gameOver;

    EventInstance gameMusicInstance;
    
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
        RuntimeManager.StudioSystem.setParameterByName("TutoDone", 0);
        gameMusicInstance = RuntimeManager.CreateInstance(gameMusic);
        gameMusicInstance.start();
        gameMusicInstance.release();
    }

    public void OnStartGame()
    {
        RuntimeManager.PlayOneShot(startGame);
        RuntimeManager.StudioSystem.setParameterByName("TutoDone", 1);
    }

    public void OnChangeMode(GameGUI.GUIMode mode)
    {
        if (mode == GameGUI.GUIMode.Death)
        {
            gameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
            gameMusicInstance.release();
            RuntimeManager.PlayOneShot(gameOver);
        }

        if (mode == GameGUI.GUIMode.MainMenu)
        {
            gameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

    }
    
 

}
