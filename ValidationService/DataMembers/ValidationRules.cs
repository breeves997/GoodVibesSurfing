using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ValidationService
{
    [Serializable]
    public class ValidationRules : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
             //info.AddValue("props", myProperty_value, typeof(string));
        }

        // The special constructor is used to deserialize values.
        public ValidationRules(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            //myProperty_value = (string)info.GetValue("props", typeof(string));
        }
    }
}
