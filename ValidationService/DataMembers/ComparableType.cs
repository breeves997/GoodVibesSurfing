using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ValidationService
{
    /// <summary>
    /// This isn't perfect. It is only good enough for my current, limited needs. It would take a decent amount of work to make this resilient.
    /// To do this right, I would want to annotate all my contracts with semver metadata to create a forward/backward compatibility resolver. But, eff that yo!
    /// </summary>
    [DataContract]
    public class ComparableType : IEquatable<ComparableType>, IComparable<ComparableType>
    {
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public Dictionary<string, string> Properties { get; set; }


        public ComparableType(Type type)
        {
            TypeName = type.AssemblyQualifiedName;
            this.Properties = Type.GetType(TypeName).GetProperties().ToDictionary(x => x.Name, x => x.GetType().FullName);
        }

        public int CompareTo(ComparableType other)
        {
            if (!Equals(other)) return -1;
            return 0;
        }

        public bool Equals(ComparableType other)
        {
            if (this.TypeName != other.TypeName) return false;
            if (!CompareX(this.Properties, other.Properties)) return false;
            return true;
        }

        public bool CompareX<TKey, TValue>(
            Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == dict2) return true;
            if ((dict1 == null) || (dict2 == null)) return false;
            if (dict1.Count != dict2.Count) return false;

            var valueComparer = EqualityComparer<TValue>.Default;

            foreach (var kvp in dict1)
            {
                TValue value2;
                if (!dict2.TryGetValue(kvp.Key, out value2)) return false;
                if (!valueComparer.Equals(kvp.Value, value2)) return false;
            }
            return true;
        }
    }
}
