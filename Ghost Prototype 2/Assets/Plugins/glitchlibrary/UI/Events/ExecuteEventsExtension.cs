using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    public static class ExecuteEventsExtension
    {
        private static readonly List<Transform> s_InternalTransformList = new List<Transform>(100);

        public static void ExecuteAllInHierarchy<T>(GameObject root, BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
        {
            s_InternalTransformList.Clear();
            GetEventChain(root, s_InternalTransformList);

            for (var i = 0; i < s_InternalTransformList.Count; i++)
            {
                var transform = s_InternalTransformList[i];
                ExecuteEvents.Execute(transform.gameObject, eventData, callbackFunction);
            }
        }

        public static void ExecuteDownInHierarchy<T>(GameObject root, BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
        {
            s_InternalTransformList.Clear();
            GetEventChainChildren(root.transform, s_InternalTransformList);

            for (var i = 0; i < s_InternalTransformList.Count; i++)
            {
                var transform = s_InternalTransformList[i];
                ExecuteEvents.Execute(transform.gameObject, eventData, callbackFunction);
            }
        }

        private static void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            if (root == null)
                return;
        
            var t = root.transform;
            while (t != null)
            {
                if (t.parent == null)
                {
                    eventChain.Add(t);
                    GetEventChainChildren(t, eventChain);
                    break;
                }
                t = t.parent;
            }
        }

        private static void GetEventChainChildren(Transform root, IList<Transform> eventChain)
        {
            foreach (Transform child in root)
            {
                eventChain.Add(child);
                GetEventChainChildren(child, eventChain);
            }
        }
    }
}