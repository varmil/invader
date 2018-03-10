using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtensions
{
    public static T RandomAt<T>(this IEnumerable<T> ie)
    {
        if (ie.Any() == false) return default(T);
        return ie.ElementAt(Random.Range(0, ie.Count()));
    }
}