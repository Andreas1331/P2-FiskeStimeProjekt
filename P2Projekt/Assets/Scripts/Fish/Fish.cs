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
    public Vector3 DesiredPoint { get; set; }
    public FishType TypeOfFish { get; set; }
    public GameObject FishObject { get; set; }
    private Rigidbody _rb;

    public Fish(int id, float weight, float movementSpeed, float maxSpeed, float width, FishType typeOfFish, GameObject preFab)
    {
        //Set variables and generate random hunger level. 
        Id = id;
        IsDead = false;
        Weight = weight;
        MovementSpeed = movementSpeed;
        MaxSpeed = maxSpeed;
        Width = width;
        Stress = 0;
        TypeOfFish = typeOfFish;
        SetRandomHunger();
        // Instantiate the physical GameObject in the world and find the needed components.
        FishObject = GameObject.Instantiate(preFab, new Vector3(Random.value*10,Random.value*2,Random.value*5), Quaternion.identity/*, GameObject.FindGameObjectWithTag("FishContainer").transform*/);
        FishObject.GetComponent<FishBehaviour>().Fish = this;
        _rb = FishObject.GetComponent<Rigidbody>();
    }

    public virtual void Swim()
    {
        if (!IsDead)
        {
            Vector3 dir = MathTools.GetDirectionVector3(FishObject.transform.position, DesiredPoint);

            DesiredPoint += dir.normalized;

            RotateTowards(dir);

            _rb.velocity = (dir.normalized * MovementSpeed);
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        Vector3 newRotDir = Vector3.RotateTowards(FishObject.transform.forward, direction, Time.deltaTime * 5, 2.5f);
        FishObject.transform.rotation = Quaternion.LookRotation(newRotDir);
    }

    public void SetRandomHunger() {
        do
        {
            Hunger = Random.value * maxHunger;
        }
        while (Hunger <= (maxHunger / 2f));
    }
}