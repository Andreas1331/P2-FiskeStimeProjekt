using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class FishBehaviorTest : FishBehaviour
    {
       


    //Rainbowtrout TestFish = new Rainbowtrout(0, 0.1f, ); //Vi skal lige finde ud af hvordan vi får en ny regnbueørred at teste med.

    // A Test behaves as an ordinary method
        [Test]
        public void TestOfcanSeeFood()
        {
            var whatever = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/Rainbowtrout.prefab", typeof (GameObject)); 
            Debug.Log(whatever);
            Rainbowtrout rainTestFish = new Rainbowtrout(0, 1f, (GameObject) whatever);
            
            //Assert.AreEqual(0, rainTestFish.Id);
        }
            //[Test]
            //public void TestOfcantSeeFoodWithoutPreviouslySeenFood()
            //{
            //    GameObject fishbehavourobject = new GameObject();
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    Assert.AreEqual(new Vector3 (), scriptTest.cantSeeFood());
            //}
            //[Test]
            //public void TestOfcantSeeFoodWithKnownFoodSpots()
            //{
            //    GameObject fishbehavourobject = new GameObject();
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    //scriptTest.lastKnownFoodSpots.Add(new Vector3(3, 0, 0));
            //    //scriptTest.lastKnownFoodSpots.Add(new Vector3(1, 0, 0));
            //    //scriptTest.lastKnownFoodSpots.Add(new Vector3(7, 0, 0));
            //    Assert.AreEqual(new Vector3(3,0, 0),scriptTest.cantSeeFood());
            //}

            //[Test]
            //public void TestOfSwimTowardsOtherFishWithOnlyOneFish() {
            //    GameObject fishbehavourobject = new GameObject();
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    GameObject otherFishObject = new GameObject();
            //    otherFishObject.AddComponent<FishBehaviour>();
            //    FishBehaviour otherFish = otherFishObject.GetComponent<FishBehaviour>();
            //    otherFish.transform.position = new Vector3(2, 0, 8);
            //    //scriptTest.nearbyFish.Add(0, otherFish);
            //    Assert.AreEqual(new Vector3(4*Mathf.Sqrt(17),0,16*Mathf.Sqrt(17)), scriptTest.SwimTowardsOtherFish());
            //}

            //[Test]
            //public void TestOfSwimTowardsOtherFishWithMultipleFish()
            //{
            //    var comparer = new Vector3EqualityComparer(0.0001f);
            //    //adding the fish from whoms "view" we are calculating
            //    GameObject fishbehavourobject = new GameObject();
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    //adding other fish
            //    GameObject otherFishObject = new GameObject();
            //    otherFishObject.AddComponent<FishBehaviour>();
            //    FishBehaviour otherFish = otherFishObject.GetComponent<FishBehaviour>();
            //    otherFish.transform.position = new Vector3(2, 0, 8);

            //    GameObject otherFishObject1 = new GameObject();
            //    otherFishObject1.AddComponent<FishBehaviour>();
            //    FishBehaviour otherFish1 = otherFishObject1.GetComponent<FishBehaviour>();
            //    otherFish1.transform.position = new Vector3(4, 0, 8);

            //    GameObject otherFishObject2 = new GameObject();
            //    otherFishObject2.AddComponent<FishBehaviour>();
            //    FishBehaviour otherFish2 = otherFishObject2.GetComponent<FishBehaviour>();
            //    otherFish2.transform.position = new Vector3(6, 0, 8);


            //    //adding the fish to the dictionary
            //    //scriptTest.nearbyFish.Add(0, otherFish);
            //    //scriptTest.nearbyFish.Add(1, otherFish1);
            //    //scriptTest.nearbyFish.Add(2, otherFish2);
            //    Assert.True(comparer.Equals(new Vector3((4 * Mathf.Sqrt(17))/3+(16*Mathf.Sqrt(5))/3+(60)/3, 0, 16 * (Mathf.Sqrt(17))/3+32*(Mathf.Sqrt(5))/3+(80.0f/3.0f)), scriptTest.SwimTowardsOtherFish()));
            //}

            //[Test]
            //public void TestOfSearchForOptimalDepth() {
            //    Vector3 fishOwnVector = new Vector3(5,15,0);
            //    GameObject fishbehavourobject = new GameObject();
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    scriptTest.transform.position = fishOwnVector;


            //    var comparer = new Vector3EqualityComparer(0.0004f);
            //    Debug.Log(scriptTest.SearchForOptimalDepth());
            //    Assert.True(comparer.Equals(scriptTest.SearchForOptimalDepth(),new Vector3(5,-15,0)));
            //}

            //[Test]
            //public void TestOfcalculateStressFactorsAloneNoObjects() {
            //    Fish.maxHunger = 1000;
            //    Fish.maxStress = 1000;
            //    GameObject fishbehavourobject = new GameObject();
            //    GameObject FishContainer = new GameObject();
            //    FishContainer.tag = "FishContainer";
            //    Debug.Log(GameObject.FindGameObjectWithTag("FishContainer").name);
            //    if (Resources.Load<GameObject>("PreFabs/Rainbowtrout") == null)
            //        Debug.Log("NULL");
            //    fishbehavourobject.AddComponent<FishBehaviour>();
            //    FishBehaviour scriptTest = fishbehavourobject.GetComponent<FishBehaviour>();
            //    scriptTest.Fish = new Rainbowtrout(0, 10f, (GameObject)Resources.Load<GameObject>("PreFabs/Rainbowtrout"));

            //    scriptTest.Fish.Hunger = 500;

            //    scriptTest.calculateStressFactorsAlone();

            //    Assert.AreEqual(0.48f, scriptTest.stressFactorsAlone.prevDirectionStress, 0.0004f);
            //    Assert.AreEqual(0.4f,scriptTest.stressFactorsAlone.findFoodStress, 0.0004f);
            //    Assert.AreEqual(0f,scriptTest.stressFactorsAlone.collisionDodgeStress, 0.0004f);
            //    Assert.AreEqual(0.96f,scriptTest.stressFactorsAlone.findFishStress, 0.0004f);
            //    Assert.AreEqual(0.16f,scriptTest.stressFactorsAlone.optimalDepthStress, 0.0004f);
            //}

        }
    }
