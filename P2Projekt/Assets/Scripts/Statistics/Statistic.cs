using System;

[Serializable]
public class Statistic
{
    public int AmountOfDeadFish;
    public float AverageHunger;
    public float AverageStress;
    public int TotalFish;
    public float TotalInsertedFood;

    public Statistic(int deadFish, float averageHunger, float averageStress, int totalFish, float totalInsertedFood)
    {
        this.AmountOfDeadFish = deadFish;
        this.AverageHunger = averageHunger;
        this.AverageStress = averageStress;
        this.TotalFish = totalFish;
        this.TotalInsertedFood = totalInsertedFood;
    }
}
