using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Mathtools;
namespace Tests
{
    public class MathToolsTestScript
    {
        MathTools mathTools = new MathTools();
        // A Test behaves as an ordinary method
        [Test]
        public void TestOfMathToolsGetDistanceBetweenVectors()
        {
            //Test around 0 vector
            Vector3 vector1 = new Vector3(0,0,0);
            Vector3 vector2 = new Vector3(0,0,0);
            Assert.AreEqual(0, mathTools.GetDistanceBetweenVectors(vector1,vector2));

            //Test with big numbers
            Vector3 bigNumberVector1 = new Vector3(33333333333333330, 22222222222222222, 5555555550);
            Vector3 bigNumberVector2 = new Vector3(55555555555555550, 111111111111111110, 10000000000);
            Assert.AreEqual(9.162457 * Mathf.Pow(10, 16), mathTools.GetDistanceBetweenVectors(bigNumberVector1, bigNumberVector2),Mathf.Pow(10,10));


            //Test with small numbers
            Vector3 smallNumberVector1 = new Vector3(-33333333333333330, -22222222222222222, -5555555550);
            Vector3 smallNumberVector2 = new Vector3(-55555555555555550, -111111111111111110, -10000000000);
            Assert.AreEqual(9.162457 * Mathf.Pow(10, 16), mathTools.GetDistanceBetweenVectors(smallNumberVector1, smallNumberVector2), Mathf.Pow(10, 10));


            // Use the Assert class to test conditions
        }

        [Test]
        public void TestOfGetAngleBetweenVectors() {
            Vector3 xVector = new Vector3(1, 0, 0);
            Vector3 yVector = new Vector3(0, 1, 0);

            Assert.AreEqual(90, mathTools.GetAngleBetweenVectors(xVector, yVector));
        }

        [Test]
        public void TestOfRadianToDegree() {
            float radiantsToTest = 3;
            Assert.AreEqual(171.8873, mathTools.RadianToDegree(radiantsToTest),0.1);
        }
        [Test]
        public void TestOfDegreeToRadian()
        {
            float degreesToTest = 171.8873f;
            Assert.AreEqual(3, mathTools.DegreeToRadian(degreesToTest), 0.0002);
        }

        [Test]
        public void TestOfGetOpposingCatheter() {
            float angle = 40; //degrees
            float length = 20;
            
            Assert.AreEqual(12.85575, mathTools.GetOpposingCatheter(angle, length), 0.1);
        }
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MathToolsTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
