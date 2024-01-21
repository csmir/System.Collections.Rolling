using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Rolling
{
    internal static class ContainedArray
    {
        public static T[] UncontainedClone<T>(Buffered<T>[] arr)
        {
            // this could be handled differently, perhaps? 
            var nArr = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                nArr[i] = arr[i].Value;
            }

            return nArr;
        }

        public static Buffered<T>[] ContainedClone<T>(T[] arr)
        {
            var nArr = new Buffered<T>[arr.Length];
            for (int i = 0; i < arr.Length; ++i)
            {
                nArr[i] = new Buffered<T>(arr[i]);
            }

            return nArr;
        }

        public static void GlobalStart<T>(Buffered<T>[] arr)
        {
            for (int i = 0; i < arr.Length ; ++i)
            { 
                arr[i].Start(); 
            }
        }
    }
}
