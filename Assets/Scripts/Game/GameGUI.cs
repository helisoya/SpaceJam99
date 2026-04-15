using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Handles the game's GUI
/// </summary>
public class GameGUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlanetInput planetInput;
    [SerializeField] private GameObject tutorialRoot;
    [SerializeField] private GameObject nameRoot;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject pauseRoot;

    [Header("Input Mode")]
    [SerializeField] private Image modeSprite;
    [SerializeField] private RectTransform modeTransform;
    [SerializeField] private Sprite[] modeSprites;
    [SerializeField] private float modeUnactiveTime = 2.0f;
    private float modeTimeRemaining;
    private bool modeVisible = false;


    [Header("Audio")]
    [SerializeField] private UnityEvent<bool> onCrankTop;
    [SerializeField] private UnityEvent<bool> onCrankBottom;
    [SerializeField] private UnityEvent onIncrementInputMode;
    [SerializeField] private UnityEvent<bool> onTutorial;
    [SerializeField] private UnityEvent onStartGame;
    [SerializeField] private UnityEvent onGiveName;

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
	/// Increments the input mode
	/// </summary>
    public void IncrementInputMode()
    {
        onIncrementInputMode.Invoke();

        if (nameRoot.activeInHierarchy && !string.IsNullOrEmpty(nameInputField.text))
        {
            onTutorial.Invoke(true);
            onGiveName.Invoke();
            nameRoot.SetActive(false);
            tutorialRoot.SetActive(true);
            planetInput.SetPlanetName(nameInputField.text);
        }
        else if (tutorialRoot.activeInHierarchy)
        {
            onTutorial.Invoke(false);
            onStartGame.Invoke();
            tutorialRoot.SetActive(false);
            planetInput.StartGame();
        }
        else if (!tutorialRoot.activeInHierarchy && !nameRoot.activeInHierarchy)
        {
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
    }

    void Update()
    {
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
