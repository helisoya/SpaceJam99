using UnityEngine;
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
        HEART
    }

    [Header("Components")]
    [SerializeField] private Planet planet;

    private bool pointerOverUI = false;
    private InputMode currentMode = InputMode.ROTATION;

    void OnLeftClick(InputValue value) // Click on the top of the crank
    {
        if (pointerOverUI)
            return;

        switch (currentMode)
        {
            case InputMode.ROTATION:
                if (value.isPressed)
                    planet.IncrementRotationSpeed();
                break;
            case InputMode.POLLUTION:
                if (value.isPressed)
                    planet.StartPollutionBrush(true);
                else
                    planet.StopPollutionBrush(true);
                break;
            case InputMode.HEAT:
                if (value.isPressed)
                    planet.SetSunPosition(true);
                break;
            case InputMode.PROJECTILE:
                if (value.isPressed)
                    planet.SetProjectileBarrierUpValue(1);
                else
                    planet.SetProjectileBarrierUpValue(0);
                break;
        }
    }

    void OnRightClick(InputValue value) // Click on the bottom of the crank
    {
        if (pointerOverUI)
            return;

        switch (currentMode)
        {
            case InputMode.POLLUTION:
                if (value.isPressed)
                    planet.StartPollutionBrush(false);
                else
                    planet.StopPollutionBrush(false);
                break;
            case InputMode.HEAT:
                if (value.isPressed)
                    planet.SetSunPosition(false);
                break;
            case InputMode.PROJECTILE:
                if (value.isPressed)
                    planet.SetProjectileBarrierDownValue(1);
                else
                    planet.SetProjectileBarrierDownValue(0);
                break;
        }
    }

    void OnMousePosition(InputValue value)
    {
    }

    /// <summary>
	/// Changes the input mode
	/// </summary>
	/// <param name="mode">The new input mode</param>
    public void ChangeMode(InputMode mode)
    {
        currentMode = mode;

        planet.EnablePollutionBrush(mode == InputMode.POLLUTION);
        planet.EnableProjectileBarrier(mode == InputMode.PROJECTILE);
    }

    void Update()
    {
        pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }
}