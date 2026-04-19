#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.VFX;
using DG.Tweening;

/// <summary>
/// Handles the planet
/// </summary>
public class Planet : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlanetData data;
    [SerializeField] private GameObject pollutionBrush;
    [SerializeField] private GameObject sunObj;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private GameObject projectileBarrier;
    [SerializeField] private Renderer planetRenderer;

    [Header("VFX")]
    [SerializeField] private ParticleSystem cleanVFX;
    [SerializeField] private ParticleSystem dripsVFX;
    [SerializeField] private ParticleSystem freezeVFX;
    [SerializeField] private ParticleSystem rotationVFX;
    [SerializeField] private VisualEffect smokeVFX;
    [SerializeField] private ParticleSystem confusedVFX;

    [Header("Audio")]
    [SerializeField] private UnityEvent<bool> onEnableCheckMode;
    [SerializeField] private UnityEvent<int> onRotationLevelChange;
    [SerializeField] private UnityEvent<int> onHappinessChange;
    [SerializeField] private UnityEvent<float> onTemperatureChange;
    [SerializeField] private UnityEvent<int> onHealthChange;
    [SerializeField] private UnityEvent onTakeDamage;
    [SerializeField] private UnityEvent onTooMuchSpeed;
    [SerializeField] private UnityEvent onTooHot;
    [SerializeField] private UnityEvent onTooCold;
    [SerializeField] private UnityEvent<bool> onCleanPollution;
    [SerializeField] private UnityEvent onFullyPolluted;
    [SerializeField] private UnityEvent onProjectileSpawn;
    [SerializeField] private UnityEvent<bool> onMoveSunInView;

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

    private bool heatIsBad;
    private float heatValue;
    private bool sunIsInFront;
    private float currentSunValue;

    private int currentHealth;
    private int currentHappiness;
    private float currentNoHappinessTimer;
    private float currentHappinessProblemTimer;
    private float currentHappinessAutoRegenTimer;
    private Stack<bool> problemsStack;

    private List<GameObject> projectiles;
    private float currentProjectileSpawnTimer;
    private float currentProjectileBarrierUpValue;
    private float currentProjectileBarrierDownValue;
    private float projectileBarrierCurrentValue;
    public bool projectileBarrierEnabled { get { return projectileBarrier.activeInHierarchy; } }

    private bool checkModeEnabled;

    private string planetName;
    private bool isActive;
    public bool isDead { get { return currentHealth == 0; } }


    void Start()
    {
        isActive = false;
        currentRotationLevel = data.defaultRotationLevel;
        currentRotationDecreaseTimer = data.secondsToRotationDecrease;

        currentHealth = data.maxHealth;
        currentHappiness = data.maxHappiness;
        currentHappinessAutoRegenTimer = data.secondsToHappinessAutoRegen;
        problemsStack = new Stack<bool>();

        pollutionActive = false;
        currentPollutionLevel = 0;
        pollutionCanIncrease = false;
        pollutionBrushActive = false;
        pollutionBrushIsTop = false;
        currentPollutionBrushValueBottom = 0;
        currentPollutionBrushValueTop = 0;
        currentPollutionBrushValue = 0;

        projectiles = new List<GameObject>();
        currentProjectileBarrierUpValue = 0;
        currentProjectileBarrierDownValue = 0;
        projectileBarrierCurrentValue = data.projectileBarrierDefaultValue;

        heatIsBad = false;
        heatValue = data.defaultHeat;
        sunIsInFront = true;
        currentSunValue = data.sunFrontPos;

        currentHappinessProblemTimer = data.secondsToHappinessDecreaseWhenProblem;

        checkModeEnabled = false;

        onRotationLevelChange.Invoke(currentRotationLevel);
        onHappinessChange.Invoke(currentHappiness);
        onHealthChange.Invoke(currentHealth);
        onTemperatureChange.Invoke(heatValue);

        GenerateNewPollutionAppearanceTimer();
        GenerateNewProjectileAppearanceTimer();

        // Update sun Position
        float posX = Mathf.Sin(Mathf.PI * 2 * currentSunValue) * data.sunOrbitRadius;
        float posZ = Mathf.Cos(Mathf.PI * 2 * currentSunValue) * data.sunOrbitRadius;

        sunObj.transform.position = data.sunOrbitCenter + new Vector3(posX, 0, posZ);

        smokeVFX.Stop();
    }

    /// <summary>
	/// Checks if the planet is active or not
	/// </summary>
	/// <returns>True if it is active</returns>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
	/// Gets the planet's name
	/// </summary>
	/// <returns>Its name</returns>
    public string GetPlanetName()
    {
        return planetName;

    }

    /// <summary>
    /// Sets the planet's name
    /// </summary>
    /// <param name="planetName">The new name</param>
    public void SetPlanetName(string planetName)
    {
        this.planetName = planetName;
    }

    /// <summary>
	/// Enables the planet
	/// </summary>
	/// <param name="isActive">True if the planet is now enabled</param>
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    /// <summary>
	/// Changes the sun's position
	/// </summary>
	/// <param name="isFront">True if the sun is in front</param>
    public void SetSunPosition(bool isFront)
    {
        onMoveSunInView.Invoke(isFront);
        sunIsInFront = isFront;
    }

    /// <summary>
	/// Generates a new random timer for the appearance of pollution
	/// </summary>
    private void GenerateNewPollutionAppearanceTimer()
    {
        currentPollutionAppearanceTimer = Random.Range(data.pollutionAppearanceTimerMin, data.pollutionAppearanceTimerMax);
    }

    /// <summary>
	/// Generates a new random timer for the appearance of projectiles
	/// </summary>
    private void GenerateNewProjectileAppearanceTimer()
    {
        currentProjectileSpawnTimer = Random.Range(data.projectileAppearanceTimerMin, data.projectileAppearanceTimerMax);
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
        if (isDead || !isActive)
            return;

        UpdateRotation();
        UpdateHappiness();
        UpdatePollution();
        UpdateHeat();
        UpdateProjectiles();
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
            rb.transform.DOScale(Vector3.zero, 1.0f).SetEase(Ease.OutQuad);
        }

        onHealthChange.Invoke(currentHealth);
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

        onHappinessChange.Invoke(currentHappiness);
    }

    /// <summary>
	/// Updates the planet's heat
	/// </summary>
    private void UpdateHeat()
    {
        float target = sunIsInFront ? data.sunFrontPos : data.sunBehindPos;
        float min = currentSunValue < target ? currentSunValue : target;
        float max = currentSunValue < target ? target : currentSunValue;
        float side = currentSunValue < target ? 1 : -1;
        currentSunValue = Mathf.Clamp(currentSunValue + Time.deltaTime * data.sunRotationSpeed * side, min, max);

        float posX = Mathf.Sin(Mathf.PI * 2 * currentSunValue) * data.sunOrbitRadius;
        float posZ = Mathf.Cos(Mathf.PI * 2 * currentSunValue) * data.sunOrbitRadius;

        sunObj.transform.position = data.sunOrbitCenter + new Vector3(posX, 0, posZ);


        heatValue += Time.deltaTime * data.heatUpdateSpeed * (sunIsInFront ? 1.0f : -1.0f);

        onTemperatureChange.Invoke(heatValue);

        if (!heatIsBad && (heatValue <= data.freezeTreshold || heatValue >= data.overheatTreshold))
        {
            bool tooHot = heatValue >= data.overheatTreshold;
            if (!tooHot)
            {
                onTooCold.Invoke();
                freezeVFX.Play();
            }
            else
            {
                onTooHot.Invoke();
                dripsVFX.Play();
            }
            heatIsBad = true;
            AddHappiness(data.heatBadHappinessMalus);
            AddProblem();

            foreach (Material material in planetRenderer.materials)
            {
                material.SetFloat("_HotLerpActive", tooHot ? 1 : 0);
                material.DOFloat(1.0f, "_LerpHot", 1.0f).SetEase(Ease.OutQuad);
                material.DOFloat(1.0f, "_LerpFrozen", 1.0f).SetEase(Ease.OutQuad);
                material.DOComplete();
            }

        }
        else if (heatIsBad && heatValue > data.freezeTreshold && heatValue < data.overheatTreshold)
        {
            heatIsBad = false;
            RemoveProblem();

            dripsVFX.Stop();
            freezeVFX.Stop();

            foreach (Material material in planetRenderer.materials)
            {
                material.DOComplete();
                material.DOFloat(0.0f, "_LerpHot", 1.0f).SetEase(Ease.OutQuad);
                material.DOFloat(0.0f, "_LerpFrozen", 1.0f).SetEase(Ease.OutQuad);
            }
        }
    }

    /// <summary>
	/// Updates the projectile
	/// </summary>
    private void UpdateProjectiles()
    {
        if (projectileBarrierEnabled)
        {
            float side = currentProjectileBarrierUpValue - currentProjectileBarrierDownValue;

            projectileBarrierCurrentValue = Mathf.Clamp(projectileBarrierCurrentValue + side * Time.deltaTime * data.projectileBarrierSpeed, 0, Mathf.PI / 2.0f);

            float posX = Mathf.Sin(data.projectileBarrierValueOffset + projectileBarrierCurrentValue) * data.projectileBarrierRadius;
            float posY = -Mathf.Cos(data.projectileBarrierValueOffset + projectileBarrierCurrentValue) * data.projectileBarrierRadius;

            projectileBarrier.transform.position = new Vector3(posX, posY, 0);
            projectileBarrier.transform.eulerAngles = new Vector3(0, 0, -45f + 90.0f * (projectileBarrierCurrentValue / (Mathf.PI / 2.0f)));
        }

        currentProjectileSpawnTimer -= Time.deltaTime;

        if (currentProjectileSpawnTimer <= 0)
        {
            GenerateNewProjectileAppearanceTimer();

            if (projectiles.Count < data.maxProjecticles)
            {
                onProjectileSpawn.Invoke();
                Projectile projectile = Instantiate(projectilePrefab);

                Vector3 position = transform.position + data.projectileSpawnOffset;
                position.y += Random.Range(-data.projectileSpawnYRange, data.projectileSpawnYRange);

                projectile.transform.position = position;
                projectile.Init((transform.position - position).normalized, data.projectileSpeed);
                projectiles.Add(projectile.gameObject);
            }
        }
    }

    /// <summary>
	/// Takes damage from a projectile
	/// </summary>
	/// <param name="obj">The projectile</param>
    public void TakeDamage(GameObject obj)
    {
        if (projectiles.Contains(obj))
        {
            onTakeDamage.Invoke();
            projectiles.Remove(obj);
            AddHappiness(-data.projectileHitHappinessMalus);
            AddHealth(-data.projectileHitHealthMalus);
            Destroy(obj);
        }
    }

    /// <summary>
	/// Updates the pollution
	/// </summary>
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
                smokeVFX.Stop();
                RemoveProblem();
                pollutionActive = false;
                GenerateNewPollutionAppearanceTimer();
                cleanVFX.Play();
            }
        }


        if (!pollutionActive)
        {
            // Waiting for timer

            currentPollutionAppearanceTimer -= Time.deltaTime;
            if (currentPollutionAppearanceTimer <= 0)
            {
                smokeVFX.Play();
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

        smokeVFX.GetAnimationCurve("AlphaClip").keys[0].value = alphaValue;
        smokeVFX.GetGradient("SmokeColors").alphaKeys[0].alpha = alphaValue;
    }

    /// <summary>
	/// Adds a problem to the problem stack
	/// </summary>
    public void AddProblem()
    {
        problemsStack.Push(true);

        if (problemsStack.Count == 1)
        {
            currentHappinessProblemTimer = data.secondsToHappinessDecreaseWhenProblem;
        }
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
        else
        {
            currentHappinessProblemTimer -= Time.deltaTime;
            if (currentHappinessProblemTimer <= 0)
            {
                currentHappinessProblemTimer = data.secondsToHappinessDecreaseWhenProblem;
                AddHappiness(-data.happinessMalusWhenProblem);
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
                confusedVFX.Play();
                RemoveProblem();
                currentRotationLevel = data.rotationLevelAfterTooMuch;
                onRotationLevelChange.Invoke(currentRotationLevel);
                currentRotationDecreaseTimer = data.secondsToRotationDecrease;
            }
            else if (currentRotationLevel > 0)
            {
                onRotationLevelChange.Invoke(currentRotationLevel);
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
        if (!checkModeEnabled)
            return;

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
        Handles.Label(new Vector3(0, currentY, 0), $"Pollution Brush state (Bottom) : {currentPollutionBrushValueBottom}"); currentY += 30;
        Handles.Label(new Vector3(0, currentY, 0), $"Heat : {heatValue}"); currentY += 30;
        Handles.Label(new Vector3(0, currentY, 0), $"Projectile Barrier value : {projectileBarrierCurrentValue}"); currentY += 20;
        Handles.Label(new Vector3(0, currentY, 0), $"Projectile Spawn in : {currentProjectileSpawnTimer}"); currentY += 30;
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

        rotationVFX.Play();

        if (currentRotationLevel == data.rotationSpeeds.Length - 1)
        {
            // Too Much
            AddProblem();
            currentRotationDecreaseTimer = data.secondsToRotationDecreaseWithTooMuch;
            AddHappiness(-data.rotationTooMuchHappinessMalus);
            onTooMuchSpeed.Invoke();
        }
        else
        {
            // Still good
            currentRotationDecreaseTimer = data.secondsToRotationDecrease;
        }

        onRotationLevelChange.Invoke(currentRotationLevel);
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
	/// Sets the projectile barrier's up value
	/// </summary>
	/// <param name="value">The new up value</param>
    public void SetProjectileBarrierUpValue(float value)
    {
        currentProjectileBarrierUpValue = value;
    }

    /// <summary>
	/// Sets the projectile barrier's down value
	/// </summary>
	/// <param name="value">The new down value</param>
    public void SetProjectileBarrierDownValue(float value)
    {
        currentProjectileBarrierDownValue = value;
    }

    /// <summary>
    /// Enables the projectile barrier
    /// </summary>
    /// <param name="enabled">True if enabled</param>
    public void EnableProjectileBarrier(bool enabled)
    {
        projectileBarrier.SetActive(enabled);
        projectileBarrierCurrentValue = data.projectileBarrierDefaultValue;
    }

    /// <summary>
	/// Starts the pollution brush
	/// </summary>
	/// <param name="top">True if it is the top side</param>
    public void StartPollutionBrush(bool top)
    {
        if (!pollutionBrushActive)
        {
            onCleanPollution.Invoke(true);
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
            onCleanPollution.Invoke(false);
            pollutionBrushActive = false;
        }
    }

    /// <summary>
	/// Enabled the check mode
	/// </summary>
	/// <param name="enabled">True if check mode is enabled</param>
    public void EnableCheckMode(bool enabled)
    {
        checkModeEnabled = enabled;
        onEnableCheckMode.Invoke(enabled);
    }
}
