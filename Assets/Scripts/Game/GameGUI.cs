using UnityEngine;

/// <summary>
/// Handles the game's GUI
/// </summary>
public class GameGUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlanetInput planetInput;

    /// <summary>
	/// Changes the input mode
	/// </summary>
	/// <param name="mode">The new input mode</param>
    public void ChangeInputMode(int mode)
    {
        planetInput.ChangeMode((PlanetInput.InputMode)mode);
    }
}
