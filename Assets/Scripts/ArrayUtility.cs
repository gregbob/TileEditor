using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrayUtility  {

    /// <summary>
    /// Converts HashSet of type T to an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set"></param>
    /// <returns></returns>
    public static T[] HashSetToArray<T>(HashSet<T> set)
    {
        T[] arr = new T[set.Count];

        var enumerator = set.GetEnumerator();

        int i = 0;
        while (enumerator.MoveNext())
        {
            arr[i] = enumerator.Current;
            i++;
        }
        return arr;
    }

    /// <summary>
    /// Make a deep copy of array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <returns></returns>
    public static T[] ArrayCopy<T>(T[] a)
    {
        T[] copy = new T[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            copy[i] = a[i];
        }
        return copy;
    }

    /// <summary>
    /// Returns index of the first element differnt between the two arrays. Returns -1 if arrays are the same.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int FindDifferentElement<T>(T[] a, T[] b)
    {
        for (int i = 0; i<a.Length; i++)
        {
            if (!a[i].Equals(b[i]))
                return i;
        }
        return -1;
    }


    /// <summary>
    /// Set idx to true. Set all others false.
    /// </summary>
    /// <param name="idx"></param>
    private void SetOnlyIndexTrue(bool[] arr, int idx)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = false;
        }
        arr[idx] = true;
    }

}
