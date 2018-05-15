using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeArena.Assets.MyScripts.Utils {
    static class ListUtil {

        private static Random random = new Random (Environment.TickCount);

        public static IList<T> Shuffle<T> (this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next (n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static bool RemoveFirst<T> (this IList<T> list, out T item, Func<T, bool> condition) {
            if (list.Any (condition)) {
                item = list.First (condition);
                list.Remove (item);
                return true;
            } else {
                item = default (T);
                return false;
            }
        }
    }
}