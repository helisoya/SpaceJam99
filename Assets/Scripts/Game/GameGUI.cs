using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the game's GUI
/// </summary>
public class GameGUI : MonoBehaviour
{
    public enum GUIMode
    {
        MainMenu,
        TransitionToNamePlanet,
        NamePlanet,
        Onboarding,
        Game,
        TransitionToDeath,
        Death
    }

    [Header("Components")]
    [SerializeField] private PlanetInput planetInput;
    [SerializeField] private Planet planet;
    [SerializeField] private GameObject mainMenuRoot;
    [SerializeField] private GameObject[] tutorialRoots;
    [SerializeField] private GameObject nameRoot;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject deathRoot;
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private Vector3 cameraPosMainMenu = new Vector3(0, 20, -10);
    [SerializeField] private Vector3 cameraPosGame = new Vector3(0, 1, -10);
    private int currentTutorialId;

    [Header("Input Mode")]
    [SerializeField] private Image modeSprite;
    [SerializeField] private RectTransform modeTransform;
    [SerializeField] private Sprite[] modeSprites;
    [SerializeField] private float modeUnactiveTime = 2.0f;
    private float modeTimeRemaining;
    private bool modeVisible = false;

    private GUIMode guiMode = GUIMode.MainMenu;


    [Header("Audio")]
    [SerializeField] private UnityEvent<bool> onCrankTop;
    [SerializeField] private UnityEvent<bool> onCrankBottom;
    [SerializeField] private UnityEvent onIncrementInputMode;
    [SerializeField] private UnityEvent<bool> onTutorial;
    [SerializeField] private UnityEvent onStartGame;
    [SerializeField] private UnityEvent onGiveName;
    [SerializeField] private UnityEvent<GUIMode> onChangeMode;

    /// <summary>
	/// Changes the input mode
	/// </summary>
	/// <param name="mode">The new input mode</param>
    public void ChangeInputMode(int mode)
    {
        planetInput.ChangeMode((PlanetInput.InputMode)mode);
    }

    /// <summary>
	/// Triggers the top crank input
	/// </summary>
	/// <param name="enabled">True if pressed</param>
    public void EnableCrankTop(bool enabled)
    {
        onCrankTop.Invoke(enabled);

        if (pauseRoot.activeInHierarchy)
            return;

        if (modeVisible)
        {
            planetInput.IncrementMode();
            modeTimeRemaining = modeUnactiveTime;
        }
        else
        {
            planetInput.InputCrankTop(enabled);
        }
    }

    /// <summary>
	/// Triggers the bottom crank input
	/// </summary>
	/// <param name="enabled">True if pressed</param>
    public void EnableCrankBottom(bool enabled)
    {
        onCrankBottom.Invoke(enabled);

        if (pauseRoot.activeInHierarchy)
            return;

        if (modeVisible)
        {
            planetInput.IncrementMode();
            modeTimeRemaining = modeUnactiveTime;
        }
        else
        {
            planetInput.InputCrankBottom(enabled);
        }
    }

    /// <summary>
	/// Enables the pause menu
	/// </summary>
	/// <param name="enabled">True if enabled</param>
    public void EnablePause(bool enabled)
    {
        pauseRoot.SetActive(enabled);
    }

    /// <summary>
	/// Gets the current GUIMode
	/// </summary>
    public GUIMode GetGUIMode()
    {
        return guiMode;
    }

    /// <summary>
	/// Increments the input mode
	/// </summary>
    public void IncrementInputMode()
    {
        onIncrementInputMode.Invoke();

        if (guiMode == GUIMode.MainMenu)
        {
            ChangeMode(GUIMode.TransitionToNamePlanet);
            mainMenuRoot.SetActive(false);
            Camera.main.transform.DOMove(cameraPosGame, 3.0f).SetEase(Ease.OutQuad).onComplete +=
            () =>
            {
                ChangeMode(GUIMode.NamePlanet);
                nameRoot.SetActive(true);
            };
        }
        else if (guiMode == GUIMode.NamePlanet && !string.IsNullOrEmpty(nameInputField.text))
        {
            ChangeMode(GUIMode.Onboarding);
            currentTutorialId = 0;
            onTutorial.Invoke(true);
            onGiveName.Invoke();
            nameRoot.SetActive(false);
            tutorialRoots[0].SetActive(true);
            planetInput.SetPlanetName(nameInputField.text);
        }
        else if (guiMode == GUIMode.Onboarding)
        {
            tutorialRoots[currentTutorialId].SetActive(false);
            currentTutorialId++;

            if (currentTutorialId >= tutorialRoots.Length)
            {
                ChangeMode(GUIMode.Game);
                onTutorial.Invoke(false);
                onStartGame.Invoke();
                planetInput.StartGame();
            }
            else
            {
                tutorialRoots[currentTutorialId].SetActive(true);
            }
        }
        else if (guiMode == GUIMode.Game)
        {
            if (pauseRoot.activeInHierarchy)
            {
                planetInput.OnPauseMenu(null);
                return;
            }

            if (modeVisible)
            {
                planetInput.IncrementMode();
                modeTimeRemaining = modeUnactiveTime;
            }
            else
            {
                modeVisible = true;
                modeTimeRemaining = modeUnactiveTime;
                modeTransform.anchoredPosition = new Vector2(0, -50);
                modeTransform.DOAnchorPos(new Vector2(0, 90), 0.3f).SetEase(Ease.InQuad);
            }
        }
        else if (guiMode == GUIMode.Death)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
	/// Changes the current ui mode
	/// </summary>
	/// <param name="mode">The current mode</param>
    private void ChangeMode(GUIMode mode)
    {
        guiMode = mode;
        onChangeMode.Invoke(mode);
    }

    /// <summary>
	/// On death callback
	/// </summary>
    private void OnDeath()
    {
        ChangeMode(GUIMode.Death);
        deathRoot.SetActive(true);
    }

    void Start()
    {
        ChangeMode(GUIMode.MainMenu);
        mainMenuRoot.SetActive(true);
        Camera.main.transform.position = cameraPosMainMenu;
    }

    void Update()
    {
        if (guiMode == GUIMode.Game && planet.isDead)
        {
            ChangeMode(GUIMode.TransitionToDeath);
            Invoke("OnDeath", 3.0f);
        }

        if (modeVisible)
        {
            modeTimeRemaining -= Time.deltaTime;
            modeSprite.sprite = modeSprites[(int)planetInput.GetInputMode()];

            if (modeTimeRemaining <= 0)
            {
                modeVisible = false;
                modeTransform.DOAnchorPos(new Vector2(0, -50), 0.3f).SetEase(Ease.InQuad);
            }
        }
    }
}
