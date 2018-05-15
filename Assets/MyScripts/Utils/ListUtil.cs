using System;
using System.Linq;
using System.Collections.Generic;

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
        
		public static T RemoveFirst<T> (this IList<T> list, Func<T, bool> condition) {
			var item = list.First (condition);
			list.Remove (item);
			return item;
		}
    }
}