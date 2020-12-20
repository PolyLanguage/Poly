using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Evaluation
{
    public interface ICurrentContext
    {
        void SetValue(string name, object value);
        object GetValue(string name);
    }

    public class DefaultContext : ICurrentContext
    {
        public Dictionary<string, object> Memory = new Dictionary<string, object>();

        public void SetValue(string name, object value)
        {
            if (GetValue(name) == null)
                Memory.Add(name, value);
            else
                ;//
        }
        public object GetValue(string name)
        {
            object res = null;
            Memory.TryGetValue(name, out res);
            return res;
        }
    }
}
