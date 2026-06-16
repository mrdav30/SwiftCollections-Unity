using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    public sealed class AdapterSwiftCollectionComponent : MonoBehaviour
    {
        [SerializeField] private SerializedSwiftList<int> _integers = new SerializedSwiftList<int>();

        public SerializedSwiftList<int> Integers => _integers;
    }
}
