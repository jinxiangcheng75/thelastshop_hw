using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CSVParserTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CSVParserTestSimplePasses()
        {
            // Use the Assert class to test conditions
            var res = CSVParser.GetConfigsFromCache<dressconfig>("heroitem_mobile", CSVParser.SEMICOLON_SPLIT);
            Debug.Log(res);

            string ss = FileUtils.loadTxtFile("Assets/Configs/test_mobile.csv");
            var list = CSVParser.DeserializeWithFieldName(ss, 2);
            //List<TestConfig> cfgs = CSVParser.ParseConfigs<TestConfig>(ss, CSVParser.FIELD_SPLIT);// = CSVParser.DeserializeByType<EquipConfig>(list, CSVParser.FIELD_SPLIT);
            //Debug.Log(cfgs);
            //Assert.AreEqual(list.Count, cfgs.Count);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator CSVParserTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
