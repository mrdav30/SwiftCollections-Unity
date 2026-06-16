using System;
using UnityEngine;

namespace SwiftCollections.Unity.Tests
{
    [Serializable]
    public struct SerializablePoint
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private string _label;

        public SerializablePoint(int id, string label)
        {
            _id = id;
            _label = label;
        }

        public int Id => _id;

        public string Label => _label;
    }
}
