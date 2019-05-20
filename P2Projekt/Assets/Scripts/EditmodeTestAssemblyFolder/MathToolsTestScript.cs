using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MathToolsTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestOfMathToolsGetDistanceBetweenVectors()
        {
            //Test around 0 vector
            Vector3 vector1 = new Vector3(0,0,0);
            Vector3 vector2 = new Vector3(0,0,0);
            Assert.AreEqual(0, MathTools.GetDistanceBetweenVectors(vector1,vector2));

            //Test with big numbers
            Vector3 bigNumberVector1 = new Vector3(0, 0, 0);
            Vector3 bigNumberVector2 = new Vector3(long.MaxValue, long.MaxValue, long.MaxValue);
            Assert.AreEqual(1.59753486f * Mathf.Pow(10, 19), MathTools.GetDistanceBetweenVectors(bigNumberVector1, bigNumberVector2),Mathf.Pow(10,15));
            
            //Test with small numbers
            Vector3 smallNumberVector1 = new Vector3(0, 0, 0);
            Vector3 smallNumberVector2 = new Vector3(long.MinValue, long.MinValue, long.MinValue);
            Assert.AreEqual(1.59753486f * Mathf.Pow(10, 19), MathTools.GetDistanceBetweenVectors(smallNumberVector1, smallNumberVector2), Mathf.Pow(10, 15));


            // Use the Assert class to test conditions
        }

        [Test]
        public void TestOfGetAngleBetweenVectors() {
            //test with 90 degree angle
            Vector3 xVector = new Vector3(1, 0, 0);
            Vector3 yVector = new Vector3(0, 1, 0);
            Assert.AreEqual(90, MathTools.GetAngleBetweenVectors(xVector, yVector));

            //test with 0 degree angle
            Vector3 xVectorzero = new Vector3(1, 0, 0);
            Vector3 yVectorzero = new Vector3(1, 0, 0);
            Assert.AreEqual(0, MathTools.GetAngleBetweenVectors(xVectorzero, yVectorzero));
        }

        [Test]
        public void TestOfRadianToDegree() {
            float radiantsToTest = 3;
            Assert.AreEqual(171.8873, MathTools.RadianToDegree(radiantsToTest),0.1);
        }
        [Test]
        public void TestOfDegreeToRadian()
        {
            float degreesToTest = 171.8873f;
            Assert.AreEqual(3, MathTools.DegreeToRadian(degreesToTest), 0.0002);
        }

        [Test]
        public void TestOfGetOpposingCatheter() {
            float angle = 40; //degrees
            float length = 20;
            
            Assert.AreEqual(12.85575, MathTools.GetOpposingCatheter(angle, length), 0.1);
        }
    }
}
