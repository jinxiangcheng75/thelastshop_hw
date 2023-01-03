using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class AssetCacheTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void AssetCacheTestSimplePasses()
        {
            // Use the Assert class to test conditions
            Logger.log("start test");
            AssetCacheTestWithEnumeratorPasses();

            Assert.AreEqual(1, 2);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator AssetCacheTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            Logger.log("test start");
            yield return null;
        }
    }
}
