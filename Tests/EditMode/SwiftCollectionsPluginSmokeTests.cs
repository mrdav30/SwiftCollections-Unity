using NUnit.Framework;

namespace SwiftCollections.Unity.Tests
{
    public class SwiftCollectionsPluginSmokeTests
    {
        [Test]
        public void SwiftDictionary_CanBeLoadedAndUsedFromUnityTestAssembly()
        {
            SwiftDictionary<string, int> dictionary = new();

            Assert.That(dictionary.Add("alpha", 1), Is.True);
            dictionary["beta"] = 2;

            Assert.That(dictionary.TryGetValue("alpha", out int value), Is.True);
            Assert.That(value, Is.EqualTo(1));
            Assert.That(dictionary["beta"], Is.EqualTo(2));
            Assert.That(dictionary.Count, Is.EqualTo(2));
        }
    }
}
