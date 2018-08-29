using System.Collections.Generic;
using UnityEngine;
namespace LARSuite
{
    public class Loom2 : MonoBehaviour
    {
        private Queue<Runable> ListQueue = new Queue<Runable>();
        private static Object locker = new Object();
        private static bool initialized;
        private static Loom2 _current;
        private static Loom2 Current
        {
            get
            {
                Initialize();
                return _current;
            }
        }



        static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("Loom2");
                _current = g.AddComponent<Loom2>();
            }
        }



        public static void RemoveAllTask()
        {
            lock (locker)
            {
                Current.ListQueue.Clear();
            }
        }

        public static void RunOnMainThread(Runable runable)
        {
            lock (locker)
            {
                Current.ListQueue.Enqueue(runable);
            }
        }
        private void Update()
        {

            Runable runable = null;
            lock (locker)
            {
                if (Current.ListQueue.Count > 0)
                {
                    runable = Current.ListQueue.Dequeue();
                }
            }
            if (runable != null)
            {
                runable.run();
            }
            runable = null;


        }

    }
}