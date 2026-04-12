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
        planetInput.InputCrankTop(enabled);
    }

    /// <summary>
	/// Triggers the bottom crank input
	/// </summary>
	/// <param name="enabled">True if pressed</param>
    public void EnableCrankBottom(bool enabled)
    {
        onCrankBottom.Invoke(enabled);
        planetInput.InputCrankBottom(enabled);
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
            planetInput.IncrementMode();
        }
    }
}
