using System.Collections.Generic;
using UnityEngine;

public class Bag<T>
{
    private readonly List<T> initialItems;
    private List<T> bag;

    public Bag(IEnumerable<T> items)
    {
        initialItems = new List<T>(items);
        Refill();
    }

    /// <summary>
    /// Gets a random item, removing it from the bag.
    /// Refills automatically when the bag is empty.
    /// </summary>
    public T GetRandom()
    {
        if (bag.Count == 0)
            Refill();

        int index = Random.Range(0, bag.Count);
        T chosen = bag[index];
        bag.RemoveAt(index);
        return chosen;
    }

    /// <summary>
    /// Clears and restores the bag to its initial values.
    /// </summary>
    public void Refill()
    {
        bag = new List<T>(initialItems);
    }

    /// <summary>
    /// Resets the bag with a new set of items.
    /// </summary>
    public void Reset(IEnumerable<T> newItems)
    {
        initialItems.Clear();
        initialItems.AddRange(newItems);
        Refill();
    }

    /// <summary>
    /// How many items are currently left in the bag.
    /// </summary>
    public int Count => bag.Count;
}
