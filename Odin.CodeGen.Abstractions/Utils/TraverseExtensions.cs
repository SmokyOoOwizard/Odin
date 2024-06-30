namespace Odin.CodeGen.Abstractions.Utils;

public static class TraverseExtensions
{
    public static IEnumerable<T> Traverse<T>(
        this IEnumerable<T> items,
        Func<T, IEnumerable<T>> childSelector)
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
}