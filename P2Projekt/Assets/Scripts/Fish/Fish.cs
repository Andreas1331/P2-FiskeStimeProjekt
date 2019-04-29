﻿using System.Collections;
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

    public Fish(int id, float weight, float movementSpeed, float maxSpeed, float width, FishType typeOfFish, GameObject preFab)
    {
        Id = id;
        IsDead = false;
        Weight = weight;
        MovementSpeed = movementSpeed;
        MaxSpeed = maxSpeed;
        Width = width;
        Stress = 0;
        Hunger = maxHunger;
        TypeOfFish = typeOfFish;
        CurrentDirection = new Vector3(0, 0, 1);
        FishObject = GameObject.Instantiate(preFab, new Vector3(Random.Range(0, 3),0,4), Quaternion.identity, GameObject.FindGameObjectWithTag("FishContainer").transform);
        FishObject.GetComponent<FishBehaviour>().Fish = this;
    }

    public virtual void MoveTowards(Vector3 direction)
    {
        Vector3 newdir = Vector3.RotateTowards(FishObject.transform.forward, direction.normalized - FishObject.transform.position, Time.deltaTime * 5, 2.5f);
        FishObject.transform.rotation = Quaternion.LookRotation(newdir);
        FishObject.transform.Translate(Vector3.Normalize(direction) * 0.5f * Time.deltaTime, Space.Self);
    }
}
