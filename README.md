# Pathfinding

An A* Pathfinding tool for Unity.

**_Classes Breakdown:_**<br />

**GraphPosition** - struct for storing x/z values for node position in the graph. Every node has one, and helper functions are used to locate nodes based on these or convert from graph to world and vice versa.<br />
**Node<T>** - base class for nodes in graph. Extend from this to create own custom nodes. Contains basic information all Nodes will share, being a GraphPosition, neighbors, and whether node is blocked. <br />
**PathNode: Node<T>** - nodes of type Node that are used in the pathfinding.
**Graph<T>** - holds all the nodes in a 2D array and has functions to handle them. Non-monobehavior and created/managed by user class. Generic that takes in a type of Node. <br />
**AStarGrid** - class responsible for creating the pathfinding grid. Inspector interaction, creating the graph, and sending/receiving data to the graph. Multiple AStarGrid's can be made to have multiple graph's in a scene of varying sizes/layouts.<br />
**Pathfinder** - Singleton responsible for finding paths on a graph. FindPath is done with a start position, end position, an AStarGrid, and an out List of Vector3s.<br />
**GraphView** - responsible for holding a grid of nodeviews and managing their display. Currently handled by the AStarGrid.<br />
**NodeView** - visual for a graph's node. Display a tile, sized to cellsize at a nodes position, and optional a text with the graph position for debugging.<br />
**StaticFormulas** - Utility class to access heuristic/traversal formulas for pathfinding.<br />
**DemoController** - Used as a demo of what is needed to do a pathfind. Reference to an AStarGrid, a unit or units to respond to the path, start and end positions, and a list of Vector3's to pass to Unit to traverse. Also showcases setting blocked/unblocked nodes.<br />
**DemoUnit** - basic unit that receives a List<Vector3> in a Move function and travels the path.<br /> 
**DemoCameraController** - basic cinemachine camera controller. WASD for movement. QE for rotation. Scroll wheel for zoom.<br />

The purpose of this tool is to be a simple but easily extensible tool to make grid based movement games. The tool should be able to drag and drop into any game, while giving simple to use customization options to get the right pathfinding feel for the various needs.

Only supports 2D square-grid based pathfinding with hex to come later when there's a need for it. The same applies for 3D graphs.

Code was designed for ease of use, extensibility, and adaptability. Classes are kept minimalistic, following good Design Patterns.

Pathfinding is done by creating a graph with a width and height and cellsize. Afterwords, a path can be found by calling into the Pathfinder Instance's FindPath method and passing in a start position, end position, the AStarGrid to find the path for, and an out list of Vector3's of the path that will be created.

The pathfinder will return a result on whether it failed, succeeded, or if the goal was unreachable. Failed means the start/end positions retrieved invalid nodes (meaning not on the graph). Unreachable goals can still be returned with _allowPartialSolution, giving a path to the closest reachable position.

Simple to use customization options include an enum to set for the type of directional movement on the graph. This would be 4 way direction (cardinal) or 8.

The Pathfinder has options to set the Heuristic(h) Algorithm to use via inspector, the Traversal Cost(g) Algorithm, Traversal Type in regards to 8-directional diagonal movement, the Heuristic Scale, partial solution returns, closed list re-evaluations for shorter path checks and start node inclusions in returned path.

Heuristic/Traversal algorithms include the Chebyshev(Chess), Diagonal, DiagonalShortcut(using 1.4f instead of sqrt(2)), Euclidean, and Manhattan.

Research was done into many A* pathfinding algorithms already in existence, including Unreal Engine's GraphAStar, RedBlobs blogs on A* and Heuristics, wikipedia, stackoverflow, AI, and others.
My algorithm shares a lot of similarities to the one used by Unreal Engine, as it demonstrated a very clean look and approach that was optimized.

Optimizations in my algorithm include bool checks for isOpen and isClosed instead of closed list creation and open/closed list checks. PriorityQueue for open list. Neighborlist creation during Graph build with blocked node exclusion to reduce node checks. Replacement of List.Insert with Reverse() for improved log. DiagonalShortcut choice for sqrt calculation avoidance if desired. 

Active Features: Map creation via inspector values or texture. Visible in scene/game through gizmos and graphview/nodeview. Moveable in editor and at runtime. Left-Click opens nodes on grid, right click sets them to blocked. Shift + Left-Click paths. If a blocked node changes while unit is moving, re-pathing is not done but can be implemented later if the need arises for constant updates while traversing a found path.

Future Implementations: Hex grid creations. 3D grid-based pathfinding.
