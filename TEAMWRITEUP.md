Team Name : Team 1
Technology : C#
Members :
Renato Da Silva
Vipul More

Algorithm:
totalSafeSquares() – Safe Reachability from (0, 0)
Objective:
Count all the reachable grid squares from the origin (0, 0), where each move must avoid "mined" cells (i.e., positions where sumDigits(x _ y) >= 19), moving only horizontally and vertically.
Algorithmic Flow:
Initialize:
A queue to perform Breadth-First Search (BFS) from (0, 0)
A HashSet to keep track of visited (x, y) positions
A dictionary or cache to remember which coordinates are "safe"
Safety Check Function (isSafe(x, y)):
Compute product = x _ y
Calculate digitSum = sum of all digits in product
Return true if digitSum < 19, else false
Cache the result for reuse
BFS Traversal:
Start from (0, 0). If it's unsafe, return 0 immediately.
While the queue is not empty:
Dequeue a coordinate (x, y)
For each direction (up, down, left, right):
Compute neighbor (nx, ny)
Skip if already visited or unsafe
Add to visited, enqueue (nx, ny)
Return:
Total number of unique safe squares visited
Diagrammatic Summary
BFS → Start at (0, 0)
↓
Enqueue safe neighbors
↓
Check isSafe(x, y)
↓
Cache result, add to visited
↓
Repeat until queue is empty

2. shortestSafeJourney(a, b, x, y) – Bidirectional A Search in Dangerous Grid*
   Objective:
   Compute the shortest path (in steps) between two coordinates (a, b) and (x, y), only moving through safe squares, using a highly optimized bidirectional A* search.
   Algorithmic Flow:
   Initialize:
   Two priority queues (startFrontier, goalFrontier) for A* from both ends
   Two Dictionary objects to track cost-so-far from start and goal (gScoreStart, gScoreGoal)
   Two HashSets or maps for visited nodes (visitedStart, visitedGoal)
   A heuristic function h(x, y) = ManhattanDistance(current, target)
   Bidirectional A Loop*:
   While both queues are not empty:
   Expand the node with the lowest f = g + h from each direction
   For each neighbor of the current node:
   If it's not safe, skip
   If it improves the known cost, update its g-score and reinsert into the queue
   If any node is found to exist in both search directions:
   Compute combined path length: gStart[node] + gGoal[node]
   Track the minimum combined cost seen
   Termination:
   If the two frontiers meet (i.e., a common node is found), return the minimum total path length
   If no meeting point is found, return -1 (no safe path exists)
   Optimizations Employed:
   Pruning of nodes whose estimated cost exceeds the best known
   Clockwise directional expansion for better CPU cache usage
   Safety lookup cache to avoid repeated dangerous square checks
   Diagrammatic Summary
   A* ← Start from (a, b) A* → Start from (x, y)
   ↓ ↓
   Expand lowest f = g + h Expand lowest f = g + h
   ↓ ↓
   Add neighbors (safe only) Add neighbors (safe only)
   ↓ ↓
   If paths meet → return gStart + gGoal
