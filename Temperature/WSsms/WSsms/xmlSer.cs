using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WSsms
{


    public static class xmlSer
   {
        private static readonly Dictionary<string, XmlSerializer> Cache = new Dictionary<string, XmlSerializer>();
 
        private static readonly object SyncRoot = new object();

       public static XmlSerializer Create(Type type, XmlRootAttribute root)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (root == null) throw new ArgumentNullException("root");
 
           var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, root.ElementName);

            lock (SyncRoot)
            {
                if (!Cache.ContainsKey(key))
                {
                    Cache.Add(key, new XmlSerializer(type, root));
                } 
            }
 
            return Cache[key];
        }
 
        public static XmlSerializer Create<T>(XmlRootAttribute root)
        {
            return Create(typeof(T), root);
        }
 
        public static XmlSerializer Create<T>()
        {
            return Create(typeof(T));
        }
 
        public static XmlSerializer Create<T>(string defaultNamespace)
        {
            return Create(typeof(T), defaultNamespace);
        }
 
        public static XmlSerializer Create(Type type)
        {
            return new XmlSerializer(type);
        }
 
        public static XmlSerializer Create(Type type, string defaultNamespace)
        {
            return new XmlSerializer(type, defaultNamespace);
        }
    }

}
