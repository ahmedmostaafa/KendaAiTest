using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace KabreetGames.TimeSystem.Utilities
{
    public static class PlayerLoopUtilities
    {

        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;
            
            var subSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (var i = 0; i < subSystemList.Count; i++)
            {
                if (subSystemList[i].type != systemToRemove.type ||
                    subSystemList[i].updateDelegate != systemToRemove.updateDelegate) continue;
                subSystemList.RemoveAt(i);
                loop.subSystemList = subSystemList.ToArray();
            }
            
            HandleSuperSystemLoopForRemove<T>(ref loop, systemToRemove);
        }

        private static void HandleSuperSystemLoopForRemove<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null || loop.subSystemList.Length == 0) return;
            
            for (var i = 0; i < loop.subSystemList.Length; i++)
            {
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
            }
        }

        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);
            
            var subSystemList = new List<PlayerLoopSystem>();
            if (loop.subSystemList != null) subSystemList.AddRange(loop.subSystemList);

            subSystemList.Insert(index, systemToInsert);
            loop.subSystemList = subSystemList.ToArray();
            return true;
        }

        private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if(loop.subSystemList == null || loop.subSystemList.Length == 0) return false;

            for (var i = 0; i < loop.subSystemList.Length; i++)
            {
                if (!InsertSystem<T>(ref loop.subSystemList[i], systemToInsert, index)) continue;
                return true;
            }
            return false;
        }
        

        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Unity Player Loop");
            sb.AppendLine("-------------------");

            foreach (var subSystem in loop.subSystemList)
            {
                PrintSubSystem(subSystem, sb, 0);
            }
            sb.AppendLine("-------------------");
            Debug.Log(sb.ToString());
        }

        private static void PrintSubSystem(PlayerLoopSystem system, StringBuilder sb, int leve)
        {
            sb.Append(' ', leve * 2).AppendLine(system.type.ToString());
            if(system.subSystemList == null || system.subSystemList.Length == 0) return;
            foreach (var subSystem in system.subSystemList)
            {
                PrintSubSystem(subSystem, sb, leve + 1);
            }
        }
    }
}