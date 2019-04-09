using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishType { Salmon = 1, RainbowTrout}

public abstract class Fish
{
    public int Id { get; set; }
    public float Weight { get; set; }
    public float MovementSpeed { get; set; }
    //public int EatTimer
    public float MaxSpeed { get; set; }
    public FishType TypeOfFish { get; set; }
    public GameObject FishObject { get; set; }
    private Rigidbody RbFish;
    public Fish(int id, float weight, float movementSpeed, float maxSpeed, FishType typeOfFish, GameObject preFab)
    {
        Id = id;
        Weight = weight;
        MovementSpeed = movementSpeed;
        MaxSpeed = maxSpeed;
        TypeOfFish = typeOfFish;
        FishObject = GameObject.Instantiate(preFab, new Vector3(), Quaternion.identity);
        RbFish = FishObject.GetComponent<Rigidbody>();
        FishObject.GetComponent<FishBehaviour>().SetFish(this);
    }

    public virtual void MoveTowards(Vector3 direction)
    {
        RbFish.AddForce(direction * MaxSpeed, ForceMode.Force);
        RbFish.GetComponent<FishBehaviour>().sumVector += direction;
    }
}
