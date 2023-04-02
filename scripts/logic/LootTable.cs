using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Game.Logic;

// TODO(calco): Refactor this so so so so much.
public partial class LootTable : Resource
{
    public const bool LogErrors = false;

    [Export] private Array<RefCounted> _keys;
    [Export] private Array<float> _weights;

    public int Count => _keys.Count;

    #region Local

    public void Add(RefCounted item, float weight)
    {
        _keys.Add(item);
        _weights.Add(weight);
    }

    // Add all items form other to this, addingup probbilities if they are the same.
    public void AddFrom(LootTable other)
    {
        for (var i = 0; i < other.Count; i++)
        {
            var item = other._keys[i];
            var weight = other._weights[i];

            if (_keys.Contains(item))
            {
                var index = _keys.IndexOf(item);
                _weights[index] += weight;
            }
            else
            {
                _keys.Add(item);
                _weights.Add(weight);
            }
        }
    }

    public void RemoveFrom(LootTable other)
    {
        for (var i = 0; i < other.Count; i++)
        {
            var item = other._keys[i];
            var weight = other._weights[i];

            if (_keys.Contains(item))
            {
                var index = _keys.IndexOf(item);
                _weights[index] -= weight;
                if (_weights[index] <= 0)
                {
                    _keys.RemoveAt(index);
                    _weights.RemoveAt(index);
                }
            }
        }
    }

    public void Clear()
    {
        _keys.Clear();
        _weights.Clear();
    }

    public IList<T> PickUnique<T>(int count) where T : RefCounted
    {
        return PickUnique<T>(count, _keys, _weights);
    }

    public IList<T> PickUniqueWithCondition<T>(int count, Func<T, bool> condition) where T : RefCounted
    {
        return PickUniqueWithCondition<T>(count, _keys, _weights, condition);
    }

    public T PickRandomWithCondition<T>(Func<T, bool> condition) where T : RefCounted
    {
        return PickRandomWithCondition<T>(_keys, _weights, condition);
    }

    public T PickRandom<T>() where T : RefCounted
    {
        return PickRandom<T>(_keys, _weights);
    }

    #endregion

    #region Static

    public static T PickRandom<T>(IList<RefCounted> keys, IList<float> weights) where T : RefCounted
    {
        return PickRandomWithCondition<T>(keys, weights, _ => true);
    }

    public static IList<T> PickUnique<T>(int count,
        IList<RefCounted> keys, IList<float> weights) where T : RefCounted
    {
        return PickUniqueWithCondition<T>(count, keys, weights, _ => true);
    }

    public static IList<T> PickUniqueWithCondition<T>(int count,
    IList<RefCounted> keys, IList<float> weights,
    Func<T, bool> condition) where T : RefCounted
    {
        var result = new List<T>();
        var keysCopy = new List<RefCounted>(keys);
        var weightsCopy = new List<float>(weights);

        for (var i = 0; i < count; i++)
        {
            var item = PickRandomWithCondition<T>(keysCopy, weightsCopy, condition);

            if (item == null)
                break;

            result.Add(item);
            keysCopy.Remove(item);
            weightsCopy.Remove(weights[keys.IndexOf(item)]);
        }

        return result;
    }

    public static T PickRandomWithCondition<T>(IList<RefCounted> keys, IList<float> weights,
    Func<T, bool> condition) where T : RefCounted
    {
        if (keys.Count != weights.Count)
        {
            if (LogErrors)
                GD.PrintErr("Keys and weights must be the same length. Something went wrong.");
            return null;
        }

        if (keys.Count == 0)
        {
            if (LogErrors)
                GD.PrintErr("Keys is empty. Cannot pick random element from empty array.");

            return null;
        }

        // Find all elements satisfying criteria.
        var filteredKeys = new List<RefCounted>();
        var filteredWeights = new List<float>();
        for (var i = 0; i < keys.Count; i++)
        {
            if (keys[i] is T item && condition(item))
            {
                filteredKeys.Add(item);
                filteredWeights.Add(weights[i]);
            }
        }

        if (filteredKeys.Count == 0)
        {
            if (LogErrors)
                GD.PrintErr("Filtred keys is empty, as no elements satisfy condition. Cannot pick random element from empty array.");

            return null;
        }

        // Pick random element from filtered keys
        var totalWeight = 0f;
        foreach (var weight in filteredWeights)
            totalWeight += weight;

        var random = new Random();
        var randomWeight = (float)random.NextDouble() * totalWeight;
        var currentWeight = 0f;
        for (var i = 0; i < filteredKeys.Count; i++)
        {
            currentWeight += filteredWeights[i];
            if (currentWeight >= randomWeight)
                return (T)filteredKeys[i];
        }

        // Should never happen.
        if (LogErrors)
            GD.PrintErr("Failed to pick random element from filtered keys.");

        return null;
    }

    #endregion
}