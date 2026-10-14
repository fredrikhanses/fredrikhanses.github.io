std::vector<Vector2DInt> Pathfinding::findPerformantPath(Vector2DInt start, Vector2DInt end)
{
	auto cmp = [](const Node& left, const Node& right) {return (left.m_FCost) > (right.m_FCost); };

	size_t mapSize = m_MapWidth * m_MapHeight;

	std::vector<int> visited = std::vector<int>(mapSize);
	std::vector<bool> walkable = std::vector<bool>(mapSize);
	std::vector<bool> water = std::vector<bool>(mapSize);
	std::vector<Node> parents = std::vector<Node>(mapSize);

	std::priority_queue<Node, std::vector<Node>, decltype(cmp)> openList(cmp);

	std::vector<Vector2DInt> path = std::vector<Vector2DInt>();
	path.reserve(mapSize);

	if (start.x == end.x && start.y == end.y)
	{
		path.push_back(end);
		return path;
	}

	Node startNode;

	startNode.x = end.x;
	startNode.y = end.y;
	startNode.m_GCost = 0;
	startNode.m_FCost = calculateHCost(end, start);
	openList.push(startNode);

	Node goalNode;
	goalNode.x = start.x;
	goalNode.y = start.y;

	const int dirx[] = { 0, 0, -1, 1, -1, 1, -1, 1 };
	const int diry[] = { -1, 1, 0, 0, 1, -1, -1, 1 };

	for (auto& sd : Map::get().m_MapSquareData)
	{
		walkable[sd.m_Position.x + sd.m_Position.y * m_MapWidth] = true;
	}
	for (auto& waterTile : Map::get().getWaterTiles())
	{
		walkable[waterTile.x + waterTile.y * m_MapWidth] = true;
		water[waterTile.x + waterTile.y * m_MapWidth] = true;
	}

	bool goalReached = false;
	while (!openList.empty())
	{
		if (openList.empty())
		{
			break;
		}

		const Node current = openList.top();
		openList.pop();

		if (current.x == goalNode.x && current.y == goalNode.y)
		{
			goalReached = true;
			break;
		}

		for (int i = 0; i < 8; ++i)
		{
			const int x = current.x + dirx[i];
			const int y = current.y + diry[i];

			if (x < 0 || y < 0 || x >= m_MapWidth || y >= m_MapHeight)
			{
				continue;
			}

			if (!walkable[x + y * m_MapWidth])
			{
				continue;
			}

			Node neighbour;
			neighbour.x = x;
			neighbour.y = y;

			const int neighbourIndex = indexOf(neighbour);

			if (visited[neighbourIndex])
			{
				continue;
			}

			neighbour.m_GCost = current.m_GCost + calculateHCost({ current.x, current.y },
				{ neighbour.x, neighbour.y });

			if (water[neighbourIndex])
			{
				neighbour.m_GCost += 10.0f;
			}

			neighbour.m_FCost = neighbour.m_GCost + calculateHCost({ neighbour.x, neighbour.y },
				{ goalNode.x, goalNode.y });

			visited[neighbourIndex] = 1;
			parents[neighbourIndex] = current;

			openList.push(neighbour);
		}
	}

	if (!goalReached)
	{
		return path;
	}

	Node currentNode = parents[indexOf(goalNode)];

	while (currentNode.x != end.x || currentNode.y != end.y)
	{
		path.push_back({ currentNode.x, currentNode.y });
		currentNode = parents[indexOf(currentNode)];
	}

	path.push_back(end);
	return path;
}
