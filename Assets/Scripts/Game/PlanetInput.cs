using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class PlanetInput : MonoBehaviour
{
    public enum InputMode
    {
        ROTATION,
        POLLUTION,
        HEAT,
        PROJECTILE,
        CHECK
    }

    [Header("Components")]
    [SerializeField] private Planet planet;
    [SerializeField] private GameGUI gui;

    private bool pointerOverUI = false;
    private InputMode currentMode = InputMode.ROTATION;
    private bool paused = false;

    void OnLeftClick(InputValue value) // Click on the top of the crank
    {
        if (pointerOverUI && !paused)
            return;

        InputCrankTop(value.isPressed);
    }

    void OnRightClick(InputValue value) // Click on the bottom of the crank
    {
        if (pointerOverUI && !paused)
            return;

        InputCrankBottom(value.isPressed);
    }

    void OnPauseMenu(InputValue value)
    {
        paused = !paused;

        planet.SetActive(!paused);
        gui.EnablePause(paused);
    }

    void OnMousePosition(InputValue value)
    {
    }

    /// <summary>
	/// Gets the current input mode
	/// </summary>
	/// <returns>The current input mode</returns>
    public InputMode GetInputMode()
    {
        return currentMode;
    }

    /// <summary>
    /// Handles the bottom crank inputs
    /// </summary>
    /// <param name="isPressed">True if the crank is now pressed</param>
    public void InputCrankBottom(bool isPressed)
    {
        if (paused)
        {
            paused = false;
            planet.SetActive(true);
            gui.EnablePause(false);
            return;
        }

        if (!planet.IsActive())
            return;

        switch (currentMode)
        {
            case InputMode.POLLUTION:
                if (isPressed)
                    planet.StartPollutionBrush(false);
                else
                    planet.StopPollutionBrush(false);
                break;
            case InputMode.HEAT:
                if (isPressed)
                    planet.SetSunPosition(false);
                break;
            case InputMode.PROJECTILE:
                if (isPressed)
                    planet.SetProjectileBarrierDownValue(1);
                else
                    planet.SetProjectileBarrierDownValue(0);
                break;
            case InputMode.CHECK:
                if (isPressed)
                    planet.EnableCheckMode(false);
                break;
        }
    }

    /// <summary>
    /// Handles the top crank inputs
    /// </summary>
    /// <param name="isPressed">True if the crank is now pressed</param>
    public void InputCrankTop(bool isPressed)
    {
        if (!planet.IsActive())
            return;

        switch (currentMode)
        {
            case InputMode.ROTATION:
                if (isPressed)
                    planet.IncrementRotationSpeed();
                break;
            case InputMode.POLLUTION:
                if (isPressed)
                    planet.StartPollutionBrush(true);
                else
                    planet.StopPollutionBrush(true);
                break;
            case InputMode.HEAT:
                if (isPressed)
                    planet.SetSunPosition(true);
                break;
            case InputMode.PROJECTILE:
                if (isPressed)
                    planet.SetProjectileBarrierUpValue(1);
                else
                    planet.SetProjectileBarrierUpValue(0);
                break;
            case InputMode.CHECK:
                if (isPressed)
                    planet.EnableCheckMode(true);
                break;
        }
    }

    /// <summary>
	/// Changes the input mode
	/// </summary>
	/// <param name="mode">The new input mode</param>
    public void ChangeMode(InputMode mode)
    {
        if (!planet.IsActive())
            return;

        if (currentMode == InputMode.CHECK)
            planet.EnableCheckMode(false);

        currentMode = mode;

        planet.EnablePollutionBrush(mode == InputMode.POLLUTION);
        planet.EnableProjectileBarrier(mode == InputMode.PROJECTILE);
    }

    /// <summary>
	/// Increments the inputMode
	/// </summary>
    public void IncrementMode()
    {
        int amount = Enum.GetValues(typeof(InputMode)).Length;
        ChangeMode((InputMode)(((int)currentMode + 1 + amount) % amount));
    }

    /// <summary>
	/// Sets the planet's name
	/// </summary>
	/// <param name="name">The planet's name</param>
    public void SetPlanetName(string name)
    {
        planet.SetPlanetName(name);
    }

    /// <summary>
	/// Starts the game
	/// </summary>
    public void StartGame()
    {
        planet.SetActive(true);
    }

    void Update()
    {
        pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }
}