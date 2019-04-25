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
        public void TestAfMathToolsGetDistanceBetweenVectors()
        {
            //Test around 0 vector
            Vector3 vector1 = new Vector3(0,0,0);
            Vector3 vector2 = new Vector3(0,0,0);
            Assert.AreEqual(mathTools.GetDistanceBetweenVectors(vector1,vector2),0);

            //Test with big numbers
            Vector3 bigNumberVector1 = new Vector3(33333333333333330, 22222222222222222, 5555555550);
            Vector3 bigNumberVector2 = new Vector3(55555555555555550, 111111111111111110, 10000000000);
            Assert.AreEqual(mathTools.GetDistanceBetweenVectors(bigNumberVector1, bigNumberVector2), 9.162457* Mathf.Pow(10,16),Mathf.Pow(10,10));


            //Test with small numbers
            Vector3 smallNumberVector1 = new Vector3(-33333333333333330, -22222222222222222, -5555555550);
            Vector3 smallNumberVector2 = new Vector3(-55555555555555550, -111111111111111110, -10000000000);
            Assert.AreEqual(mathTools.GetDistanceBetweenVectors(smallNumberVector1, smallNumberVector2), 9.162457 * Mathf.Pow(10, 16), Mathf.Pow(10, 10));


            // Use the Assert class to test conditions
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
