using System.Collections.Generic;

public static class ListPool<T>
{
    private static readonly ObjectPool<List<T>> s_listPool = new ObjectPool<List<T>>(null, Clear);

    public static List<T> Get()
    {
        return s_listPool.Get();
    }

    public static List<T> GetFromSource(List<T> sourceList)
    {
        var targetList = s_listPool.Get();
        foreach (var e in sourceList)
        {
            targetList.Add(e);
        }
        return targetList;
    }

    public static void Release(List<T> toRelease)
    {
        s_listPool.Release(toRelease);
    }

    private static void Clear(List<T> list)
    { 
        list.Clear(); 
    }
}
