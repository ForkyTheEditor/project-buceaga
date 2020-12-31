using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


/// <summary>
/// Dictionary-like data structure used for two way mapping of types. For instance: dict[T] = K; and dict[K] = T;
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="K"></typeparam>
public class TwoWayMappingDictionary<T, K>
{
    //Data storage
    private Dictionary<T, K> _firstDict;
    private Dictionary<K, T> _secondDict;

    public TwoWayMappingDictionary()
    {
        //Can't have the two types be the same as it would create errors
        if(typeof(T) == typeof(K))
        {
            throw new Exception("Generic types cannot be the same!");
        }

        _firstDict = new Dictionary<T, K>();
        _secondDict = new Dictionary<K, T>();
    }

    /// <summary>
    /// Add two mapped values to the dictionary. For instance: 1 - DoorObject; "Map" - MapObject;
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Add(T first, K second)
    {
        _firstDict.Add(first, second);
        _secondDict.Add(second, first);
    }

    //Indexers
    public K this[T index]
    {
        get 
        {
            if (_firstDict.ContainsKey(index))
            {
                return _firstDict[index];
            }

            throw new ArgumentException("Parameter key does not have a corresponding value!");

        }

        set { _firstDict[index] = value; }
    }

    public T this[K index]
    {
        get
        {
            if (_secondDict.ContainsKey(index))
            {
                return _secondDict[index];
            }

            throw new ArgumentException("Parameter key does not have a corresponding value!");

        }

        set { _secondDict[index] = value; }
    }

}
