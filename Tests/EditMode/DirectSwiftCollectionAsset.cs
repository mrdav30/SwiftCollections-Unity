using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public sealed class DirectSwiftCollectionAsset : ScriptableObject
    {
        [SerializeField]
        private SwiftList<int> _integers = new SwiftList<int>();

        [SerializeField]
        private SwiftList<SerializablePoint> _points = new SwiftList<SerializablePoint>();

        [SerializeField]
        private SwiftList<UnityEngine.Object> _objectReferences = new SwiftList<UnityEngine.Object>();

        [SerializeField]
        private SwiftDictionary<string, int> _dictionary = new SwiftDictionary<string, int>();

        public SwiftList<int> Integers => _integers;

        public SwiftList<SerializablePoint> Points => _points;

        public SwiftList<UnityEngine.Object> ObjectReferences => _objectReferences;

        public SwiftDictionary<string, int> Dictionary => _dictionary;

        public void SetIntegers(int[] items)
        {
            _integers = new SwiftList<int>(items);
        }

        public void SetPoints(SerializablePoint[] items)
        {
            _points = new SwiftList<SerializablePoint>(items);
        }

        public void SetObjectReferences(UnityEngine.Object[] items)
        {
            _objectReferences = new SwiftList<UnityEngine.Object>(items);
        }

        public void SetDictionary(params (string Key, int Value)[] items)
        {
            _dictionary = new SwiftDictionary<string, int>(items.Length);
            for (int i = 0; i < items.Length; i++)
                _dictionary.Add(items[i].Key, items[i].Value);
        }
    }
}
