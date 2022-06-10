using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtension
    {
        public static IEnumerable<T> Traverse<T>(this T item, Func<T,T> childSelector)
        {
            var stack = new Stack<T>(new T[]{ item });

            while (stack.Any())
            {
                var next = stack.Pop();
                if (next != null)
                {
                    yield return next;
                    stack.Push(childSelector(next));
                }
            }
        }

        public static IEnumerable<T> Traverse<T>(this T item, Func<T,IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(new T[]{ item });

            while (stack.Any())
            {
                var next = stack.Pop();
                //if(next != null)
                //{
                yield return next;
                foreach (var child in childSelector(next))
                {
                    stack.Push(child);
                }
                //}
            }
        }

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
            }
        }

        public static IEnumerable<IEnumerable<T>> Traverse<T>(this IEnumerable<T> items, Func<IEnumerable<T>, T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<IEnumerable<T>>(new IEnumerable<T>[]{ items });
            while (stack.Any())
            {
                var next = stack.Pop();
                if (next != null)
                {
                    yield return next;

                    foreach (var item in next)
                    {
                        stack.Push(childSelector(next, item));
                    }    
                }
            }
        }

        public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source, System.Func<TSource, bool> predicate)
        {
            var result = source.Where(predicate);
            var count = result.Count();
            if (count != 0)
            {
                var index = UnityEngine.Random.Range(0, count);
                return result.ElementAt(index);
            }
            else
            {
                return default(TSource);
            }
        }


        public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var count = source.Count();
            if (count != 0)
            {
                var index = UnityEngine.Random.Range(0, count);
                return source.ElementAt(index);
            }
            else
            {
                return default(TSource);
            }
        }
    }
}
