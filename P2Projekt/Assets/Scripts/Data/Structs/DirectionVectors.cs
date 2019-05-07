using UnityEngine;

public struct DirectionVectors
{
    public Vector3 PreviousPoint { get; set; }
    public Vector3 FindFoodDirection { get; set; }
    public Vector3 SwimWithOrToFish { get; set; }
    public Vector3 DodgeCollisionDirection { get; set; }
    public Vector3 OptimalDepthDirection { get; set; }
    public Vector3 HoldDistanceToFishDirection { get; set; }
}