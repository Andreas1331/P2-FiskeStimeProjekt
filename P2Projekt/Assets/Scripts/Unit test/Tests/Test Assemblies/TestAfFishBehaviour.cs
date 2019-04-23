using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestAfFishBehaviour
    {
        // A Test behaves as an ordinary method
        
        
        
            // Use the Assert class to test conditions
        
        [Test]
        public void TestAfSearchForOptimalDepth()
        {
            float y = 5;
            float x = -2.5f;
            Vector3 result = SearchForOptimalDepth(y, x);

            Assert.AreEqual(new Vector3(0, 0, 0),result);

        }
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestAfTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }



        private Vector3 SearchForOptimalDepth(float y, float x)
        {
            var vec = new Vector3(0, -y / 2 - x, 0);

            return vec;
        }
    }
}
