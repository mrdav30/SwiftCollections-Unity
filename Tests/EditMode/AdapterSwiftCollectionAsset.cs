using SwiftCollections.Unity;
using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public sealed class AdapterSwiftCollectionAsset : ScriptableObject
    {
        [SerializeField]
        private SerializedSwiftList<int> _integers = new SerializedSwiftList<int>();

        [SerializeField]
        private SerializedSwiftList<SerializablePoint> _points = new SerializedSwiftList<SerializablePoint>();

        [SerializeField]
        private SerializedSwiftList<UnityEngine.Object> _objectReferences = new SerializedSwiftList<UnityEngine.Object>();

        [SerializeField]
        private SerializedSwiftDictionary<string, int> _dictionary = new SerializedSwiftDictionary<string, int>();

        [SerializeField]
        private SerializedSwiftArray2D<int> _array2D = new SerializedSwiftArray2D<int>();

        [SerializeField]
        private SerializedSwiftArray3D<int> _array3D = new SerializedSwiftArray3D<int>();

        [SerializeField]
        private SerializedSwiftSparseSet _sparseSet = new SerializedSwiftSparseSet();

        [SerializeField]
        private SerializedSwiftSparseMap<int> _sparseMap = new SerializedSwiftSparseMap<int>();

        [SerializeField]
        private SerializedSwiftBiDictionary<string, int> _biDictionary = new SerializedSwiftBiDictionary<string, int>();

        public SerializedSwiftList<int> Integers => _integers;

        public SerializedSwiftList<SerializablePoint> Points => _points;

        public SerializedSwiftList<UnityEngine.Object> ObjectReferences => _objectReferences;

        public SerializedSwiftDictionary<string, int> Dictionary => _dictionary;

        public SerializedSwiftArray2D<int> Array2D => _array2D;

        public SerializedSwiftArray3D<int> Array3D => _array3D;

        public SerializedSwiftSparseSet SparseSet => _sparseSet;

        public SerializedSwiftSparseMap<int> SparseMap => _sparseMap;

        public SerializedSwiftBiDictionary<string, int> BiDictionary => _biDictionary;
    }
}
