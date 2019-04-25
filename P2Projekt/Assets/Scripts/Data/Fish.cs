using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishType { Salmon = 1, RainbowTrout}

public abstract class Fish 
{
    public int Id { get; set; }
    public bool IsDead { get; set; }
    public float Weight { get; set; }
    public float MovementSpeed { get; set; }
    public float MaxSpeed { get; set; }
    public float Width { get; set; }
    private float _stress;
    public float Stress
    {
        get { return _stress; }
        set
        {
            _stress = value;
            if (_stress > 1000)
                _stress = 1000;
            else if (_stress < 0)
                _stress = 0;
        }
    }
    public static float maxHunger;
    public static float maxStress;

    private float _hunger;
    public float Hunger
    {
        get { return _hunger; }
        set
        {
            _hunger = value;
            if (_hunger > 1000)
                _hunger = 1000;
            else if (_hunger < 0)
                _hunger = 0;
        }
    }
    public float Size { get; set; }
    public Vector3 CurrentDirection { get; set; }
    public FishType TypeOfFish { get; set; }
    public GameObject FishObject { get; set; }
    //public int EatTimer

    public Fish(int id, float weight, float movementSpeed, float maxSpeed, float width, FishType typeOfFish, GameObject preFab)
    {
        Id = id;
        IsDead = false;
        Weight = weight;
        MovementSpeed = movementSpeed;
        MaxSpeed = maxSpeed;
        Width = width;
        Stress = 0;
        Hunger = 1000;
        TypeOfFish = typeOfFish;
        CurrentDirection = new Vector3(0, 0, 1);
        FishObject = GameObject.Instantiate(preFab, new Vector3(Random.Range(0,10), Random.Range(0,10), Random.Range(0,10)), Quaternion.identity, GameObject.FindGameObjectWithTag("FishContainer").transform);
        FishObject.GetComponent<FishBehaviour>().Fish = this;
    }

    public virtual void MoveTowards(Vector3 direction)
    {
        //Debug.Log(direction);
        FishObject.transform.Translate(direction * MovementSpeed * Time.deltaTime, Space.Self);
        //FishObject.transform.position = Vector3.MoveTowards(FishObject.transform.position, direction, MovementSpeed * Time.deltaTime);
        //RbFish.AddForce(direction * MaxSpeed, ForceMode.Force);
        //RbFish.GetComponent<FishBehaviour>().sumVector += direction;
    }
}
