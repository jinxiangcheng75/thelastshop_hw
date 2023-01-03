using System.Collections.Generic;
using System.Linq;

public static partial class ListExstensions
{
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static T GetRandomElement<T>(this IEnumerable<T> list)
    {
        return list.Shuffle().First();
    }

    public static List<T> Shuffle<T>(this IEnumerable<T> list)
    {
        return list.OrderBy(arg => System.Guid.NewGuid()).ToList();
    }
}