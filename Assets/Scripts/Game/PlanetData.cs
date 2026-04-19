using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Lamentin/PlanetData")]
public class PlanetData : ScriptableObject
{
    [Header("Health")]
    public int maxHappiness = 100;
    public int maxHealth = 100;
    public float secondsToHealthDecreaseNoHappiness = 1.0f;
    public int healthMalusNoHappiness = 1;
    public float secondsToHappinessDecreaseWhenProblem = 1.0f;
    public int happinessMalusWhenProblem = 1;
    public float secondsToHappinessAutoRegen = 1.0f;
    public int happinessAutoRegenAmount = 1;
    public int rotationNotEnoughLevel = 4;
    public int rotationNotEnoughHappinessMalus = 5;

    [Header("Check")]
    public float secondsToCheckModeEmotion = 20.0f;


    [Header("Rotation")]
    public float[] rotationSpeeds;
    public float secondsToRotationDecrease = 20.0f;
    public float secondsToRotationDecreaseWithTooMuch = 7.0f;
    public int rotationLevelAfterTooMuch = 6;
    public int defaultRotationLevel = 4;
    public float rotationTransitionSpeed = 5.0f;
    public int rotationTooMuchHappinessMalus = 10;

    [Header("Pollution")]
    [Range(1.0f, 120.0f)] public float pollutionAppearanceTimerMin = 35.0f;
    [Range(1.0f, 120.0f)] public float pollutionAppearanceTimerMax = 45.0f;
    public float maxPollutionLevel = 100.0f;
    public float pollutionIncreasePerSeconds = 5.0f;
    public int pollutionHappinessMalus = 20;
    public int pollutionHealthMalus = 10;
    public float pollutionBrushSpeed = 0.5f;
    public float pollutionBrushNotActiveSpeed = 10.0f;
    public int pollutionSideCleanHappinessBonus = 5;

    [Header("Heat")]
    public Vector3 sunOrbitCenter = Vector3.zero;
    public float sunOrbitRadius = 30.0f;
    public float sunBehindPos = 0.5f;
    public float sunFrontPos = 0.0f;
    public float defaultHeat = 50.0f;
    public float overheatTreshold = 90.0f;
    public float freezeTreshold = 10.0f;
    public float heatUpdateSpeed = 1.0f / 5.0f;
    public int heatBadHappinessMalus = 10;
    public float sunRotationSpeed = 1.0f;

    [Header("Projectiles")]
    [Range(1.0f, 120.0f)] public float projectileAppearanceTimerMin = 20.0f;
    [Range(1.0f, 120.0f)] public float projectileAppearanceTimerMax = 40.0f;
    public int maxProjecticles = 2;
    public float projectileSpeed = 1.0f;
    public int projectileHitHappinessMalus = 20;
    public int projectileHitHealthMalus = 10;
    public Vector3 projectileSpawnOffset;
    [Range(1.0f, 50.0f)] public float projectileSpawnYRange = 3.0f;
    public float projectileBarrierRadius = 2.75f;
    public float projectileBarrierDefaultValue = Mathf.PI / 4.0f;
    public float projectileBarrierValueOffset = Mathf.PI / 4.0f;
    public float projectileBarrierSpeed = 0.5f;
}
