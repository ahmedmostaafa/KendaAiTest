using System;
using KendaAi.BlackboardSystem.Util;

namespace KendaAi.BlackboardSystem
{
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        readonly string name;
        readonly int hashedKey;


        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey = name.ComputeFnv1AHash();
        }

        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;

        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }
}