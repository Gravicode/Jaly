using System;
using System.Collections.Generic;
using System.Text;

namespace DKI.Bot.App.Helpers
{
    public class ObjectContainer
    {
        private static Dictionary<Type, object> MyContainers = new Dictionary<Type, object>();

        public static void Register<T>(T data)
        {
            if(!MyContainers.ContainsKey(typeof(T)))
                MyContainers.Add(typeof(T), data);
        }

        public static T Get<T>()
        {
            if (MyContainers.ContainsKey(typeof(T)))
                return (T)MyContainers[typeof(T)];
            else return default(T);
        }

    }
}
