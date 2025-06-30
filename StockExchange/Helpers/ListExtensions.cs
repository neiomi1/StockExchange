
public static class ListExtensions
{
    /// <summary>
    /// Returns a list of k items selected randomly from the source list using Selection Sampling.
    /// </summary>
    public static List<T> SelectionSample<T>(this IList<T> source, int k, Random rng = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        k = Math.Max(k, 0);
        k = Math.Min(k, source.Count);

        rng ??= new Random();
        var result = new List<T>(k);

        int needed = k;
        int left = source.Count;

        for (int i = 0; i < source.Count; i++)
        {
            double prob = (double)needed / left;
            if (rng.NextDouble() < prob)
            {
                result.Add(source[i]);
                needed--;
                if (needed == 0)
                    break;
            }
            left--;
        }

        return result;
    }
}