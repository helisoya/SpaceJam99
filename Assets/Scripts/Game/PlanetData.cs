using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Lamentin/PlanetData")]
public class PlanetData : ScriptableObject
{
    [Header("Health")]
    public int maxHappiness = 100;
    public int maxHealth = 100;
    public float secondsToHealthDecreaseNoHappiness = 1.0f;
    public int healthMalusNoHappiness = 1;
    public float secondsToHappinessAutoRegen = 1.0f;
    public int happinessAutoRegenAmount = 1;
    public int rotationNotEnoughLevel = 4;
    public int rotationNotEnoughHappinessMalus = 5;


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
    public float secondsToMaxPollution = 10.0f;
    public int pollutionHappinessMalus = 20;
    public int pollutionHealthMalus = 10;
    public float pollutionBrushSpeed = 0.5f;
    public float pollutionBrushNotActiveSpeed = 10.0f;
    public int pollutionSideCleanHappinessBonus = 5;
}
