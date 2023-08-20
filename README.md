# Pathfinding

A work in progress A* pathfinding tool for use in Unity.

The purpose of this tool was to be a simple but easily extensible tool to make grid based movement games. The tool should be able to drag and drop into any game, while giving simple to use customization options to get the right pathfinding feel for the various needs.

Only supports 2D square-grid based pathfinding with hex to come later when there's a need for it. The same applies for 3D graphs.

Code was designed for ease of use, extensibility, and adaptability. Classes are kept minimalistic, following good Design Patterns.

Pathfinding is done by creating a graph with a width and height. Afterwords, a path can be found by calling into the Pathfinder Instance's FindPath method and passing in a start node, end node, the graph, and a list of nodes to be returned.
The pathfinder will return a result on whether it failed, succeeded, or if the goal was reachable.

Simple to use customization options include an enum to set for the type of directional movement on the graph. This would be 4 way direction or 8.

The Pathfinder has options to set the Heuristic(h) Algorithm to use via inspector, the Traversal Cost(g) Algorithm, Traversal Type in regards to 8-directional diagonal movement, the Heuristic Scale, partial solution returns, closed list re-evaluations for shorter path checks and start node inclusions in returned path.

Heuristic/Traversal algorithms include the Chebyshev(Chess), Diagonal, DiagonalShortcut(using 1.4f instead of sqrt(2)), Euclidean, and Manhattan.

Research was done into many A* pathfinding algorithms already in existence, including Unreal Engine's GraphAStar, RedBlobs blogs on A* and Heuristics, wikipedia, stackoverflow, AI, and others.
My algorithm shares a lot of similarities to the one used by Unreal Engine, as it demonstrated a very clean look and approach that was optimized.

Optimizations in my algorithm include bool checks for isOpen and isClosed instead of closed list creation and open/closed list checks. PriorityQueue for open list. Neighborlist creation during Graph build with blocked node exclusion to reduce node checks. Replacement of List.Insert with Reverse() for improved log. DiagonalShortcut choice for sqrt calculation avoidance if desired. 

Current features include map creation via text or texture image, reading in values to set blocked nodes and even terrain. Left-lick and path. Right-click and waypoint path.

Future implementations will be a basic unit with selection. Grid creation outside of textures/text files. Better graph and nodeview implementations while keeping with MVC pattern. Possible splitting of the Node class to have pathfinding nodes and nodes used for game unit information. Top down camera and movement.
