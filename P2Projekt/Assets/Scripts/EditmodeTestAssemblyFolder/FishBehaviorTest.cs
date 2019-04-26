using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FishBehaviorTest
    {
        
        
        //Rainbowtrout TestFish = new Rainbowtrout(0, 0.1f, ); //Vi skal lige finde ud af hvordan vi får en ny regnbueørred at teste med.
        
        // A Test behaves as an ordinary method
        [Test]
        public void TestOfcanSeeFood()
        {
            GameObject fishbehavourobject = new GameObject();
            fishbehavourobject.AddComponent<FishBehaviour>();
            FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            scriptTest.knownFoodSpots[0] = new Vector3(10,10,10);
            scriptTest.knownFoodSpots[1] = new Vector3(0,10,0);
            scriptTest.knownFoodSpots[2] = new Vector3(10,12,13);
            scriptTest.knownFoodSpots[3] = new Vector3(30,20,10);
            Assert.AreEqual(scriptTest.knownFoodSpots[1], scriptTest.canSeeFood());
        }
        [Test]
        public void TestOfcantSeeFoodWithoutPreviouslySeenFood()
        {
            GameObject fishbehavourobject = new GameObject();
            fishbehavourobject.AddComponent<FishBehaviour>();
            FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            Assert.AreEqual(new Vector3 (), scriptTest.cantSeeFood());
        }
        [Test]
        public void TestOfcantSeeFoodWithKnownFoodSpots()
        {
            GameObject fishbehavourobject = new GameObject();
            fishbehavourobject.AddComponent<FishBehaviour>();
            FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            scriptTest.MathTools = new Mathtools.MathTools();
            scriptTest.lastKnownFoodSpots.Add(new Vector3(3, 0, 0));
            scriptTest.lastKnownFoodSpots.Add(new Vector3(1, 0, 0));
            scriptTest.lastKnownFoodSpots.Add(new Vector3(7, 0, 0));
            Assert.AreEqual(new Vector3(3,0, 0),scriptTest.cantSeeFood());
        }

        [Test]
        public void TestOfSwimTowardsOtherFishWithOnlyOneFish() {
            GameObject fishbehavourobject = new GameObject();
            fishbehavourobject.AddComponent<FishBehaviour>();
            FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            scriptTest.MathTools = new Mathtools.MathTools();
            GameObject otherFishObject = new GameObject();
            otherFishObject.AddComponent<FishBehaviour>();
            FishBehaviour otherFish = otherFishObject.GetComponent<FishBehaviour>();
            otherFish.transform.position = new Vector3(2, 0, 8);
            scriptTest.nearbyFish.Add(0, otherFish);
            Assert.AreEqual(new Vector3(4*Mathf.Sqrt(17),0,16*Mathf.Sqrt(17)), scriptTest.SwimTowardsOtherFish());
        }



        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FishBehaviorTEstWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
