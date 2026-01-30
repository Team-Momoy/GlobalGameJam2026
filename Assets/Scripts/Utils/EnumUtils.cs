using System;

public static class EnumUtils
{
    public static T GetRandomEnumValue<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}