using System;
using System.Collections.Generic;

public class SafeCoordinates
{
    // Checks if a coordinate is safe to visit
    public static bool IsSafe(int x, int y)
    {
        int product = x * y;
        int sum = 0;
        foreach (char c in Math.Abs(product).ToString())
            sum += c - '0';
        return sum < 19;
    }

    // Calculates the total number of safe squares reachable from (0,0)
    public static int TotalSafeSquares()
    {
        var visited = new HashSet<(int, int)>();
        var queue = new Queue<(int, int)>();
        queue.Enqueue((0, 0));
        visited.Add((0, 0));
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            for (int d = 0; d < 4; d++)
            {
                int nx = x + dx[d], ny = y + dy[d];
                var next = (nx, ny);
                if (!visited.Contains(next) && IsSafe(nx, ny))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }
        return visited.Count;
    }

    // Finds the shortest safe path from (a, b) to (x, y)
    public static int ShortestSafeJourney(int a, int b, int x, int y)
    {
        if (!IsSafe(a, b) || !IsSafe(x, y)) return -1;
        var visited = new HashSet<(int, int)>();
        var queue = new Queue<((int, int), int)>();
        queue.Enqueue(((a, b), 0));
        visited.Add((a, b));
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (pos, steps) = queue.Dequeue();
            if (pos == (x, y)) return steps;
            for (int d = 0; d < 4; d++)
            {
                int nx = pos.Item1 + dx[d], ny = pos.Item2 + dy[d];
                var next = (nx, ny);
                if (!visited.Contains(next) && IsSafe(nx, ny))
                {
                    visited.Add(next);
                    queue.Enqueue((next, steps + 1));
                }
            }
        }
        return -1;
    }

    // Example usage and test cases
    public static void Main()
    {
        Console.WriteLine(IsSafe(3, 4));      // True
        Console.WriteLine(IsSafe(67, 43));    // False
        Console.WriteLine(IsSafe(96, -69));   // True
        Console.WriteLine(IsSafe(123, 456));  // False

        Console.WriteLine("Total Safe Squares: " + TotalSafeSquares());

        Console.WriteLine("Shortest Safe Journey (0,0) to (3,4): " + ShortestSafeJourney(0, 0, 3, 4));
        Console.WriteLine("Shortest Safe Journey (0,0) to (67,43): " + ShortestSafeJourney(0, 0, 67, 43));
    }
}