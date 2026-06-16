using NUnit.Framework;
using SwiftCollections.Unity;
using UnityEditor;
using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public class SwiftCollectionsUnitySerializationTests
    {
        private const string TestFolder = "Assets/SwiftCollectionsSerializationTests";

        [SetUp]
        public void SetUp()
        {
            DeleteTestFolder();
            AssetDatabase.CreateFolder("Assets", "SwiftCollectionsSerializationTests");
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestFolder();
        }

        [Test]
        public void DirectSwiftListIntField_DoesNotPersistThroughScriptableObjectAsset()
        {
            DirectSwiftCollectionAsset asset = ScriptableObject.CreateInstance<DirectSwiftCollectionAsset>();
            asset.SetIntegers(new[] { 1, 2, 3 });

            DirectSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "direct-int-list.asset");

            Assert.That(GetCountOrZero(loaded.Integers), Is.EqualTo(0));
        }

        [Test]
        public void DirectSwiftListSerializableStructField_DoesNotPersistThroughScriptableObjectAsset()
        {
            DirectSwiftCollectionAsset asset = ScriptableObject.CreateInstance<DirectSwiftCollectionAsset>();
            asset.SetPoints(new[]
            {
                new SerializablePoint(1, "one"),
                new SerializablePoint(2, "two")
            });

            DirectSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "direct-struct-list.asset");

            Assert.That(GetCountOrZero(loaded.Points), Is.EqualTo(0));
        }

        [Test]
        public void DirectSwiftListObjectReferenceField_DoesNotPersistThroughScriptableObjectAsset()
        {
            ReferencedObjectAsset reference = CreateReferencedObjectAsset();
            DirectSwiftCollectionAsset asset = ScriptableObject.CreateInstance<DirectSwiftCollectionAsset>();
            asset.SetObjectReferences(new UnityEngine.Object[] { reference });

            DirectSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "direct-object-list.asset");

            Assert.That(GetCountOrZero(loaded.ObjectReferences), Is.EqualTo(0));
        }

        [Test]
        public void DirectSwiftDictionaryStringIntField_DoesNotPersistThroughScriptableObjectAsset()
        {
            DirectSwiftCollectionAsset asset = ScriptableObject.CreateInstance<DirectSwiftCollectionAsset>();
            asset.SetDictionary(("one", 1), ("two", 2));

            DirectSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "direct-dictionary.asset");

            Assert.That(GetCountOrZero(loaded.Dictionary), Is.EqualTo(0));
        }

        [Test]
        public void SerializedSwiftListIntField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.Integers.SetItems(new[] { 4, 5, 6 });

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-int-list.asset");

            Assert.That(loaded.Integers.Count, Is.EqualTo(3));
            Assert.That(loaded.Integers[0], Is.EqualTo(4));
            Assert.That(loaded.Integers[1], Is.EqualTo(5));
            Assert.That(loaded.Integers[2], Is.EqualTo(6));
            Assert.That(loaded.Integers.ToSwiftList().ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        }

        [Test]
        public void SerializedSwiftListSerializableStructField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.Points.SetItems(new[]
            {
                new SerializablePoint(7, "seven"),
                new SerializablePoint(8, "eight")
            });

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-struct-list.asset");

            Assert.That(loaded.Points.Count, Is.EqualTo(2));
            Assert.That(loaded.Points[0].Id, Is.EqualTo(7));
            Assert.That(loaded.Points[0].Label, Is.EqualTo("seven"));
            Assert.That(loaded.Points[1].Id, Is.EqualTo(8));
            Assert.That(loaded.Points[1].Label, Is.EqualTo("eight"));
        }

        [Test]
        public void SerializedSwiftListObjectReferenceField_RoundTripsThroughScriptableObjectAsset()
        {
            ReferencedObjectAsset reference = CreateReferencedObjectAsset();
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.ObjectReferences.SetItems(new UnityEngine.Object[] { reference });

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-object-list.asset");

            Assert.That(loaded.ObjectReferences.Count, Is.EqualTo(1));
            Assert.That(loaded.ObjectReferences[0], Is.SameAs(reference));
        }

        [Test]
        public void SerializedSwiftDictionaryStringIntField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.Dictionary.SetItems(
                new SerializedSwiftDictionaryEntry<string, int>("one", 1),
                new SerializedSwiftDictionaryEntry<string, int>("two", 2));

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-dictionary.asset");

            Assert.That(loaded.Dictionary.Count, Is.EqualTo(2));
            Assert.That(loaded.Dictionary.TryGetValue("one", out int one), Is.True);
            Assert.That(loaded.Dictionary.TryGetValue("two", out int two), Is.True);
            Assert.That(one, Is.EqualTo(1));
            Assert.That(two, Is.EqualTo(2));
            Assert.That(loaded.Dictionary.ToSwiftDictionary().Count, Is.EqualTo(2));
        }

        [Test]
        public void SerializedSwiftArray2DIntField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.Array2D.SetItems(2, 3, 1, 2, 3, 4, 5, 6);

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-array2d.asset");

            Assert.That(loaded.Array2D.Width, Is.EqualTo(2));
            Assert.That(loaded.Array2D.Height, Is.EqualTo(3));
            Assert.That(loaded.Array2D[0, 0], Is.EqualTo(1));
            Assert.That(loaded.Array2D[0, 2], Is.EqualTo(3));
            Assert.That(loaded.Array2D[1, 0], Is.EqualTo(4));
            Assert.That(loaded.Array2D.ToSwiftArray2D()[1, 2], Is.EqualTo(6));
        }

        [Test]
        public void SerializedSwiftArray3DIntField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.Array3D.SetItems(2, 2, 2, 1, 2, 3, 4, 5, 6, 7, 8);

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-array3d.asset");

            Assert.That(loaded.Array3D.Width, Is.EqualTo(2));
            Assert.That(loaded.Array3D.Height, Is.EqualTo(2));
            Assert.That(loaded.Array3D.Depth, Is.EqualTo(2));
            Assert.That(loaded.Array3D[0, 1, 1], Is.EqualTo(4));
            Assert.That(loaded.Array3D[1, 0, 1], Is.EqualTo(6));
            Assert.That(loaded.Array3D.ToSwiftArray3D()[1, 1, 1], Is.EqualTo(8));
        }

        [Test]
        public void SerializedSwiftSparseSetField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.SparseSet.SetItems(2, 5, 9);

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-sparse-set.asset");

            Assert.That(loaded.SparseSet.Count, Is.EqualTo(3));
            Assert.That(loaded.SparseSet.Contains(2), Is.True);
            Assert.That(loaded.SparseSet.Contains(5), Is.True);
            Assert.That(loaded.SparseSet.Contains(9), Is.True);
            Assert.That(loaded.SparseSet.ToSwiftSparseSet().Contains(5), Is.True);
        }

        [Test]
        public void SerializedSwiftSparseMapField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.SparseMap.SetItems(
                new SerializedSwiftSparseMapEntry<int>(3, 30),
                new SerializedSwiftSparseMapEntry<int>(7, 70));

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-sparse-map.asset");

            Assert.That(loaded.SparseMap.Count, Is.EqualTo(2));
            Assert.That(loaded.SparseMap.TryGetValue(3, out int three), Is.True);
            Assert.That(loaded.SparseMap.TryGetValue(7, out int seven), Is.True);
            Assert.That(three, Is.EqualTo(30));
            Assert.That(seven, Is.EqualTo(70));
            Assert.That(loaded.SparseMap.ToSwiftSparseMap().ContainsKey(7), Is.True);
        }

        [Test]
        public void SerializedSwiftBiDictionaryField_RoundTripsThroughScriptableObjectAsset()
        {
            AdapterSwiftCollectionAsset asset = ScriptableObject.CreateInstance<AdapterSwiftCollectionAsset>();
            asset.BiDictionary.SetItems(
                new SerializedSwiftBiDictionaryEntry<string, int>("left", 10),
                new SerializedSwiftBiDictionaryEntry<string, int>("right", 20));

            AdapterSwiftCollectionAsset loaded = SaveAndReloadAsset(asset, "adapter-bi-dictionary.asset");

            Assert.That(loaded.BiDictionary.Count, Is.EqualTo(2));
            Assert.That(loaded.BiDictionary.TryGetValue("left", out int left), Is.True);
            Assert.That(loaded.BiDictionary.TryGetKey(20, out string rightKey), Is.True);
            Assert.That(left, Is.EqualTo(10));
            Assert.That(rightKey, Is.EqualTo("right"));
            Assert.That(loaded.BiDictionary.ToSwiftBiDictionary().ContainsValue(20), Is.True);
        }

        private static T SaveAndReloadAsset<T>(T asset, string fileName)
            where T : ScriptableObject
        {
            string path = $"{TestFolder}/{fileName}";
            AssetDatabase.CreateAsset(asset, path);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Resources.UnloadAsset(asset);

            T loaded = AssetDatabase.LoadAssetAtPath<T>(path);
            Assert.That(loaded, Is.Not.Null);
            return loaded;
        }

        private static ReferencedObjectAsset CreateReferencedObjectAsset()
        {
            ReferencedObjectAsset reference = ScriptableObject.CreateInstance<ReferencedObjectAsset>();
            reference.name = "Referenced Object";
            AssetDatabase.CreateAsset(reference, $"{TestFolder}/referenced-object.asset");
            AssetDatabase.SaveAssets();
            return reference;
        }

        private static int GetCountOrZero<T>(SwiftList<T> list)
        {
            return list == null ? 0 : list.Count;
        }

        private static int GetCountOrZero<TKey, TValue>(SwiftDictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return dictionary == null ? 0 : dictionary.Count;
        }

        private static void DeleteTestFolder()
        {
            if (AssetDatabase.IsValidFolder(TestFolder))
                AssetDatabase.DeleteAsset(TestFolder);
        }

    }
}
