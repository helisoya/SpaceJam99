#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles the planet
/// </summary>
public class Planet : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Renderer pollutionRenderer;
    [SerializeField] private PlanetData data;
    [SerializeField] private GameObject pollutionBrush;

    private int currentRotationLevel;
    private float currentRotationDecreaseTimer;

    private bool pollutionActive;
    private bool pollutionCanIncrease;
    private float currentPollutionLevel;
    private float currentPollutionAppearanceTimer;

    private bool pollutionBrushActive;
    private bool pollutionBrushIsTop;
    private float currentPollutionBrushValueTop;
    private float currentPollutionBrushValueBottom;
    private float currentPollutionBrushValue;
    public bool pollutionBrushEnabled { get { return pollutionBrush.activeInHierarchy; } }

    private int currentHealth;
    private int currentHappiness;
    private float currentNoHappinessTimer;
    private float currentHappinessAutoRegenTimer;
    private Stack<bool> problemsStack;

    public bool isDead { get { return currentHealth == 0; } }


    void Start()
    {
        problemsStack = new Stack<bool>();
        currentRotationLevel = data.defaultRotationLevel;
        currentRotationDecreaseTimer = data.secondsToRotationDecrease;
        currentHealth = data.maxHealth;
        currentHappiness = data.maxHappiness;
        pollutionActive = false;
        currentPollutionLevel = 0;
        pollutionCanIncrease = false;
        currentHappinessAutoRegenTimer = data.secondsToHappinessAutoRegen;
        pollutionBrushActive = false;
        pollutionBrushIsTop = false;
        currentPollutionBrushValueBottom = 0;
        currentPollutionBrushValueTop = 0;
        currentPollutionBrushValue = 0;
        GenerateNewPollutionAppearanceTimer();
    }

    /// <summary>
	/// Generates a new random timer for the appearance of pollution
	/// </summary>
    private void GenerateNewPollutionAppearanceTimer()
    {
        currentPollutionAppearanceTimer = Random.Range(data.pollutionAppearanceTimerMin, data.pollutionAppearanceTimerMax);
    }

    /// <summary>
	/// Gets the rotation speed level
	/// </summary>
	/// <returns>The rotation speed level</returns>
    public int GetRotationLevel()
    {
        return currentRotationLevel;
    }

    void Update()
    {
        if (isDead)
            return;

        // Rotation Speed Checks

        UpdateRotation();
        UpdateHappiness();
        UpdatePollution();
    }

    /// <summary>
	/// Adds health to the planet
	/// </summary>
	/// <param name="amount">The health amount to add</param>
    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, data.maxHealth);
        if (currentHealth == 0)
        {
            // Dead
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    /// <summary>
	/// Adds happiness to the planet
	/// </summary>
	/// <param name="amount">The hapiness amount</param>
    public void AddHappiness(int amount)
    {
        int lastValue = currentHappiness;
        currentHappiness = Mathf.Clamp(currentHappiness + amount, 0, data.maxHappiness);

        if (lastValue != currentHappiness && currentHappiness == 0) // Reset timer only when getting to 0 the first time
            currentNoHappinessTimer = data.secondsToHealthDecreaseNoHappiness;
    }

    private void UpdatePollution()
    {
        if (pollutionBrushEnabled)
        {
            if (pollutionBrushActive)
            {
                currentPollutionBrushValue = Mathf.Clamp(currentPollutionBrushValue + Time.deltaTime * data.pollutionBrushSpeed, 0.0f, 1.0f + Time.deltaTime * data.pollutionBrushSpeed);
                if (currentPollutionBrushValue >= 1.0f)
                    currentPollutionBrushValue -= 1.0f;


                if (pollutionActive)
                {
                    pollutionCanIncrease = false;
                    if (pollutionBrushIsTop && currentPollutionBrushValueTop < 1.0f)
                    {
                        currentPollutionBrushValueTop = Mathf.Clamp(currentPollutionBrushValueTop + Time.deltaTime * data.pollutionBrushSpeed, 0.0f, 1.0f);
                        if (currentPollutionBrushValueTop >= 1.0f)
                            AddHappiness(data.pollutionSideCleanHappinessBonus);
                    }
                    else if (!pollutionBrushIsTop && currentPollutionBrushValueBottom < 1.0f)
                    {
                        currentPollutionBrushValueBottom = Mathf.Clamp(currentPollutionBrushValueBottom + Time.deltaTime * data.pollutionBrushSpeed, 0.0f, 1.0f);
                        if (currentPollutionBrushValueBottom >= 1.0f)
                            AddHappiness(data.pollutionSideCleanHappinessBonus);
                    }
                }
            }
            else
            {
                currentPollutionBrushValue = Mathf.Clamp(currentPollutionBrushValue - Time.deltaTime * data.pollutionBrushNotActiveSpeed, 0.0f, 1.0f);
            }

            // Set Brush Position

            float posX = Mathf.Sin(Mathf.PI * 2 * currentPollutionBrushValue);
            float posY;
            if (pollutionBrushIsTop)
                posY = -Mathf.Cos(Mathf.PI * 2 * currentPollutionBrushValue) + 1;
            else
                posY = Mathf.Cos(Mathf.PI * 2 * currentPollutionBrushValue) - 1;

            pollutionBrush.transform.position = new Vector3(posX, posY, pollutionBrush.transform.position.z);

            if (currentPollutionBrushValueBottom >= 1.0f && currentPollutionBrushValueTop >= 1.0f && pollutionActive)
            {
                // All clean
                RemoveProblem();
                pollutionActive = false;
                GenerateNewPollutionAppearanceTimer();
            }
        }


        if (!pollutionActive)
        {
            // Waiting for timer

            currentPollutionAppearanceTimer -= Time.deltaTime;
            if (currentPollutionAppearanceTimer <= 0)
            {
                currentPollutionLevel = 0.0f;
                currentPollutionBrushValueBottom = 0;
                currentPollutionBrushValueTop = 0;
                pollutionActive = true;
                pollutionCanIncrease = true;
                AddProblem();
            }
        }
        else
        {
            // Pollution rising
            if (pollutionCanIncrease)
            {
                currentPollutionLevel = Mathf.Clamp(currentPollutionLevel + Time.deltaTime * data.pollutionIncreasePerSeconds, 0.0f, data.maxPollutionLevel);
                if (currentPollutionLevel >= data.maxPollutionLevel)
                {
                    pollutionCanIncrease = false;
                    AddHappiness(-data.pollutionHappinessMalus);
                    AddHealth(-data.pollutionHealthMalus);
                }
            }
        }

        float alphaValue = (currentPollutionLevel / data.maxPollutionLevel)
            * (1.0f - (currentPollutionBrushValueTop + currentPollutionBrushValueBottom) / 2.0f);

        pollutionRenderer.material.SetColor("_BaseColor", new Color(1.0f, 1.0f, 1.0f, alphaValue));
    }

    /// <summary>
	/// Adds a problem to the problem stack
	/// </summary>
    public void AddProblem()
    {
        problemsStack.Push(true);
    }

    /// <summary>
	/// Removes a problem from the problem stack
	/// </summary>
    public void RemoveProblem()
    {
        if (problemsStack.Count > 0)
        {
            problemsStack.Pop();
            if (problemsStack.Count == 0)
                currentHappinessAutoRegenTimer = data.secondsToHappinessAutoRegen;
        }
    }

    /// <summary>
    /// Updates the happiness
    /// </summary>
    private void UpdateHappiness()
    {
        if (problemsStack.Count == 0)
        {
            currentHappinessAutoRegenTimer -= Time.deltaTime;
            if (currentHappinessAutoRegenTimer <= 0)
            {
                currentHappinessAutoRegenTimer = data.secondsToHappinessAutoRegen;
                AddHappiness(data.happinessAutoRegenAmount);
            }
        }

        if (currentHappiness == 0)
        {
            currentNoHappinessTimer -= Time.deltaTime;

            if (currentNoHappinessTimer <= 0)
            {
                currentNoHappinessTimer = data.secondsToHealthDecreaseNoHappiness;
                AddHealth(-data.healthMalusNoHappiness);
            }
        }
    }

    /// <summary>
	/// Updates the rotation
	/// </summary>
    private void UpdateRotation()
    {
        Vector3 targetSpeed = new Vector3(0, data.rotationSpeeds[currentRotationLevel], 0);
        rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, targetSpeed, data.rotationTransitionSpeed * Time.deltaTime);

        currentRotationDecreaseTimer -= Time.deltaTime;
        if (currentRotationDecreaseTimer <= 0)
        {
            if (currentRotationLevel == data.rotationSpeeds.Length - 1)
            {
                RemoveProblem();
                currentRotationLevel = data.rotationLevelAfterTooMuch;
                currentRotationDecreaseTimer = data.secondsToRotationDecrease;
            }
            else if (currentRotationLevel > 0)
            {
                currentRotationLevel--;
                currentRotationDecreaseTimer = data.secondsToRotationDecrease;
                if (currentRotationLevel <= data.rotationNotEnoughLevel)
                {
                    AddHappiness(-data.rotationNotEnoughHappinessMalus);
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        Handles.BeginGUI();
        int currentY = 0;
        Handles.Label(new Vector3(0, currentY, 0), $"Health : {currentHealth}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Happiness : {currentHappiness}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"No Happiness Health Decrease Timer : {currentNoHappinessTimer}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Happiness Auto Regen Timer : {currentHappinessAutoRegenTimer}"); currentY += 30;
        Handles.Label(new Vector3(0, currentY, 0), $"Current Rotation Level : {currentRotationLevel}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Rotation Level decrease in : {currentRotationDecreaseTimer}"); currentY += 30;
        Handles.Label(new Vector3(0, currentY, 0), $"Current Pollution Level : {currentPollutionLevel}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Appearance Timer : {currentPollutionAppearanceTimer}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush value : {currentPollutionBrushValue}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush visible : {pollutionBrushEnabled}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush active : {pollutionBrushActive}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush state (Top) : {currentPollutionBrushValueTop}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush state (Bottom) : {currentPollutionBrushValueBottom}"); currentY += 20;
        Handles.EndGUI();
    }
#endif

    /// <summary>
    /// Increments the rotation speed level
    /// </summary>
    public void IncrementRotationSpeed()
    {
        if (isDead)
            return;

        if (currentRotationLevel < data.rotationSpeeds.Length - 1)
            currentRotationLevel++;

        if (currentRotationLevel == data.rotationSpeeds.Length - 1)
        {
            // Too Much
            AddProblem();
            currentRotationDecreaseTimer = data.secondsToRotationDecreaseWithTooMuch;
            AddHappiness(-data.rotationTooMuchHappinessMalus);
        }
        else
        {
            // Still good
            currentRotationDecreaseTimer = data.secondsToRotationDecrease;
        }
    }

    /// <summary>
	/// Enables the pollution brush
	/// </summary>
	/// <param name="enabled">True if enabled</param>
    public void EnablePollutionBrush(bool enabled)
    {
        pollutionBrush.SetActive(enabled);
        currentPollutionBrushValue = 0;
    }

    /// <summary>
	/// Starts the pollution brush
	/// </summary>
	/// <param name="top">True if it is the top side</param>
    public void StartPollutionBrush(bool top)
    {
        if (!pollutionBrushActive)
        {
            pollutionBrushActive = true;
            pollutionBrushIsTop = top;
        }
    }

    /// <summary>
	/// Stops the pollution brush
	/// </summary>
	/// <param name="top">True if it is the top side</param>
    public void StopPollutionBrush(bool top)
    {
        if (pollutionBrushActive && pollutionBrushIsTop == top)
        {
            pollutionBrushActive = false;
        }
    }
}
