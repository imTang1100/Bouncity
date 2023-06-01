using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "scriptableObjects/Gameplay Settings", order = 1)]
public class GameplaySettings : ScriptableObject
{
    public float forcePerSecond;
    public float ballDiameter;
    public float ballDrag;
    public float defaultBounciness;
    public float ballSpawnDistance;
    public float ballSpawnHeight;
    public float holeDistance;
    public float holeAngle;
    public float holeRadius;
    public float holeSpawnPadding;
    public float buildingOpacityDefault;
    public float buildingOpacityOnHit;
    public float floorOffset;
    public float obstacleHoleMargin;
    public float obstalceBallMargin;

    public event Action OnUpdated;

    public void PublishUpdates()
    {
        OnUpdated?.Invoke();
    }
}
