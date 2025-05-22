using System;
using System.Collections.Generic;

namespace ConsoleApp1;

public static class SafeCoordinates
{
    private static readonly HashSet<(int, int)> SafeCache = new();
    private static readonly HashSet<(int, int)> UnsafeCache = new();

    // Checks if a coordinate is safe to visit
    public static bool IsSafe(int x, int y)
    {
        // Fast path for zero
        if (x == 0 || y == 0) return true;

        long product = (long)x * y;
        int sum = 0;

        // Faster than string conversion
        product = Math.Abs(product);
        while (product > 0 && sum < 19)
        {
            sum += (int)(product % 10);
            product /= 10;
        }

        return sum < 19;
    }

    // Calculates the total number of safe squares reachable from (0,0)
    public static int TotalSafeSquares()
    {
        try
        {
            const int startX = 0, startY = 0;
            if (!IsSafe(startX, startY)) return 0;

            const int GRID_LIMIT = 9999;
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();

            // Preallocate buffer capacity for better performance
            visited.EnsureCapacity(GRID_LIMIT);

            queue.Enqueue((startX, startY));
            visited.Add((startX, startY));

            // Stack-allocated array for directions
            Span<(int dx, int dy)> directions = stackalloc (int, int)[]
            {
            (0, 1), (0, -1), (1, 0), (-1, 0)
        };

            while (queue.TryDequeue(out var current))
            {
                foreach (var (dx, dy) in directions)
                {
                    int nx = current.Item1 + dx;
                    int ny = current.Item2 + dy;

                    // Combined bounds check
                    if ((uint)(nx + GRID_LIMIT) > (uint)(2 * GRID_LIMIT) ||
                        (uint)(ny + GRID_LIMIT) > (uint)(2 * GRID_LIMIT))
                        continue;

                    var next = (nx, ny);

                    // Skip if already visited
                    if (!visited.Add(next))
                        continue;

                    // Check cache first, then compute safety
                    if (SafeCache.Contains(next) ||
                        (!UnsafeCache.Contains(next) && CheckAndCacheSafety(next)))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return visited.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 0;
        }

    }

    private static bool CheckAndCacheSafety((int x, int y) coord)
    {
        if (IsSafe(coord.x, coord.y))
        {
            SafeCache.Add(coord);
            return true;
        }
        UnsafeCache.Add(coord);
        return false;
    }

    // Finds the shortest safe path from (a, b) to (x, y)
    public static int ShortestSafeJourney(int a, int b, int x, int y)
    {
        // Early validation with cache check
        if (!CheckAndCacheSafety((a, b)) || !CheckAndCacheSafety((x, y)))
            return -1;

        const int GRID_LIMIT = 9999;

        // Priority queues for forward and backward search
        var forwardQueue = new PriorityQueue<((int x, int y) pos, int steps), int>();
        var backwardQueue = new PriorityQueue<((int x, int y) pos, int steps), int>();

        var forwardVisited = new Dictionary<(int, int), int>();
        var backwardVisited = new Dictionary<(int, int), int>();

        var start = (a, b);
        var target = (x, y);

        // Initialize bidirectional search
        forwardQueue.Enqueue((start, 0), ManhattanDistance(start, target));
        backwardQueue.Enqueue((target, 0), ManhattanDistance(target, start));
        forwardVisited[start] = 0;
        backwardVisited[target] = 0;

        // Stack-allocated directions
        Span<(int dx, int dy)> directions = stackalloc (int, int)[]
        {
            (1, 0), (0, 1), (-1, 0), (0, -1) // Clockwise order for cache locality
        };

        int bestPath = int.MaxValue;

        while (forwardQueue.Count > 0 && backwardQueue.Count > 0)
        {
            // Process forward search
            if (ProcessNextNode(forwardQueue, forwardVisited, backwardVisited,
                directions, target, GRID_LIMIT, ref bestPath))
                break;

            // Process backward search
            if (ProcessNextNode(backwardQueue, backwardVisited, forwardVisited,
                directions, start, GRID_LIMIT, ref bestPath))
                break;
        }

        return bestPath == int.MaxValue ? -1 : bestPath;
    }

    private static bool ProcessNextNode(
        PriorityQueue<((int x, int y) pos, int steps), int> queue,
        Dictionary<(int, int), int> visited,
        Dictionary<(int, int), int> oppositeVisited,
        Span<(int dx, int dy)> directions,
        (int x, int y) target,
        int gridLimit,
        ref int bestPath)
    {
        var ((x, y), steps) = queue.Dequeue();
        var currentPos = (x, y);

        // Check if we found a better path through opposite direction
        if (oppositeVisited.TryGetValue(currentPos, out int oppositeSteps))
        {
            int totalPath = steps + oppositeSteps;
            if (totalPath < bestPath)
            {
                bestPath = totalPath;
                return true;
            }
        }

        // Skip if we've found a better path
        if (steps >= bestPath) return false;

        foreach (var (dx, dy) in directions)
        {
            int nx = x + dx;
            int ny = y + dy;

            if ((uint)(nx + gridLimit) > (uint)(2 * gridLimit) ||
                (uint)(ny + gridLimit) > (uint)(2 * gridLimit))
                continue;

            var next = (nx, ny);
            int newSteps = steps + 1;

            // Skip if we've found this position with a better path
            if (visited.TryGetValue(next, out int existingSteps) && existingSteps <= newSteps)
                continue;

            // Check safety using cache
            if (!SafeCache.Contains(next) &&
                (UnsafeCache.Contains(next) || !CheckAndCacheSafety(next)))
                continue;

            visited[next] = newSteps;
            int priority = newSteps + ManhattanDistance(next, target);
            queue.Enqueue((next, newSteps), priority);
        }

        return false;
    }

    private static int ManhattanDistance((int x, int y) a, (int x, int y) b)
        => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

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