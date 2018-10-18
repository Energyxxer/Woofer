using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Saves
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PersistentPropertyAttribute : Attribute
    {
        public string KeyName;

        public PersistentPropertyAttribute() : this(null) { }
        public PersistentPropertyAttribute(string keyName) => KeyName = keyName;
    }
}
