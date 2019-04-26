using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct lambdaStructAlone
{
    public float prevDirectionLambda { get; set; }
    public float findFoodLambda { get; set; }
    public float findOtherFishLambda { get; set; }
    public float collisionDodgeLambda { get; set; }
    public float optimalDepthLambda { get; set; }
}

public struct lambdaStructSchool
{
    public float prevDirectionLambda { get; set; }
    public float findFoodLambda { get; set; }
    public float swimWithOtherFishLambda { get; set; }
    public float collisionDodgeLambda { get; set; }
    public float optimalDepthLambda { get; set; }
    public float holdDistanceToFishLambda { get; set; }
}
public struct stressFactorLambdaAlone
{
    public float prevDirectionStress { get; set; }
    public float findFoodStress { get; set; }
    public float findFishStress { get; set; }
    public float collisionDodgeStress { get; set; }
    public float optimalDepthStress { get; set; }
}
public struct stressFactorLambdaSchool
{
    public float prevDirectionStress { get; set; }
    public float findFoodStress { get; set; }
    public float swimWithOtherFishStress { get; set; }
    public float collisionDodgeStress { get; set; }
    public float optimalDepthStress { get; set; }
    public float holdDistanceToFishStress { get; set; }
}
public struct hungerFactorLambdaAlone
{
    public float prevDirectionHunger { get; set; }
    public float findFoodHunger { get; set; }
    public float findFishHunger { get; set; }
    public float collisionDodgeHunger { get; set; }
    public float optimalDepthHunger { get; set; }
}
public struct hungerFactorLambdaSchool
{
    public float prevDirectionHunger { get; set; }
    public float findFoodHunger { get; set; }
    public float swimWithOtherFishHunger { get; set; }
    public float collisionDodgeHunger { get; set; }
    public float optimalDepthHunger { get; set; }
    public float holdDistanceToFishHunger { get; set; }
}
public struct depthFactorLambdaAlone
{
    public float prevDirectionDepth { get; set; }
    public float findFoodDepth { get; set; }
    public float findFishDepth { get; set; }
    public float collisionDodgeDepth { get; set; }
    public float optimalDepthDepth { get; set; }
}
public struct depthFactorLambdaSchool
{
    public float prevDirectionDepth { get; set; }
    public float findFoodDepth { get; set; }
    public float swimWithOtherFishDepth { get; set; }
    public float collisionDodgeDepth { get; set; }
    public float optimalDepthDepth { get; set; }
    public float holdDistanceToFishDepth { get; set; }
}
public struct directionVectors
{
    public Vector3 previousDirection { get; set; }
    public Vector3 findFoodDirection { get; set; }
    public Vector3 swimWithOrToFish { get; set; }
    public Vector3 dodgeCollisionDirection { get; set; }
    public Vector3 optimalDepthDirection { get; set; }
    public Vector3 holdDistanceToFishDirection { get; set; }

}
