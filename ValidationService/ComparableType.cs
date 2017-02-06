using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationService
{
    /// <summary>
    /// This isn't perfect. It is only good enough for my current, limited needs. It would take a decent amount of work to make this resilient.
    /// To do this right, I would want to annotate all my contracts with semver metadata to create a forward/backward compatibility resolver. But, eff that yo!
    /// </summary>
    public class ComparableType : IEquatable<ComparableType>, IComparable<ComparableType>
    {
        private readonly Type Type;

        public ComparableType(Type type)
        {
            Type = type;
        }

        public int CompareTo(ComparableType other)
        {
            if (!Equals(other)) return -1;
            return 0;
        }

        public bool Equals(ComparableType other)
        {
            var thisProps = Type.GetProperties();
            var thatProps = other.Type.GetProperties();
            if (thisProps.Count() != thatProps.Count()) return false;
            if (Type.Name != other.Type.Name) return false;
            if (Type.Namespace != other.Type.Namespace) return false;
            if (!CheckMatchingProps(thisProps, thatProps)) return false;
            return true;
        }

        private bool CheckMatchingProps(PropertyInfo[] current, PropertyInfo[] other)
        {
            var currentList =current.ToList();
            var otherList = other.ToList();

            var samesies = currentList.Zip(otherList, (x, y) => x.Name == y.Name && x.PropertyType == y.PropertyType && x.Attributes == y.Attributes);
            return samesies.All(x => x == true);
        }
    }
}
