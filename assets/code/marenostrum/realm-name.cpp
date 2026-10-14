void UIText::adjustText()
{
	if (m_OwnedRegionIDs.size() > 0)
	{
		m_Conquered = false;
		Map& map = Map::get();
		std::vector<SquareData>& mapData = map.m_MapSquareData;
		Vector2DInt topLeftMostPosition = { map.width, map.height };
		Vector2DInt topRightMostPosition = { 0, map.height };
		Vector2DInt bottomLeftMostPosition = { map.width, 0 };
		Vector2DInt bottomRightMostPosition = { 0, 0 };
		Vector2DInt realmPositions = { 0, 0 };
		unsigned int numberOfRealmPositions = 0;
		for (SquareData& square : mapData)
		{
			for (unsigned int regionId : m_OwnedRegionIDs)
			{
				if (regionId == square.m_RegionID)
				{
					realmPositions += square.m_Position;
					numberOfRealmPositions++;

					if (square.m_Position.x + square.m_Position.y * map.m_AspectRatio
						<= topLeftMostPosition.x + topLeftMostPosition.y * map.m_AspectRatio)
					{
						topLeftMostPosition = square.m_Position;
					}

					if (square.m_Position.x - square.m_Position.y * map.m_AspectRatio
						> topRightMostPosition.x - topRightMostPosition.y * map.m_AspectRatio)
					{
						if (square.m_Position.x - topRightMostPosition.x
							>= square.m_Position.y * map.m_AspectRatio - topRightMostPosition.y * map.m_AspectRatio)
						{
							topRightMostPosition = square.m_Position;
						}
					}

					if (square.m_Position.x - square.m_Position.y * map.m_AspectRatio
						< bottomLeftMostPosition.x - bottomLeftMostPosition.y * map.m_AspectRatio)
					{
						if (square.m_Position.x - bottomLeftMostPosition.x
							<= square.m_Position.y * map.m_AspectRatio - bottomLeftMostPosition.y * map.m_AspectRatio)
						{
							bottomLeftMostPosition = square.m_Position;
						}
					}

					if (square.m_Position.x + square.m_Position.y * map.m_AspectRatio
						>= bottomRightMostPosition.x + bottomRightMostPosition.y * map.m_AspectRatio)
					{
						bottomRightMostPosition = square.m_Position;
					}
				}
			}
		}

		Vector2D topLeftMostPositionScreen = map.convertToScreen(topLeftMostPosition);
		Vector2D topRightMostPositionScreen = map.convertToScreen(topRightMostPosition);
		Vector2D bottomLeftMostPositionScreen = map.convertToScreen(bottomLeftMostPosition);
		Vector2D bottomRightMostPositionScreen = map.convertToScreen(bottomRightMostPosition);

		calculatePosition(realmPositions, numberOfRealmPositions);

		Vector2D diagonal = calculateDiagonal(topLeftMostPositionScreen, bottomRightMostPositionScreen,
			bottomLeftMostPositionScreen, topRightMostPositionScreen);

		calculateSize(diagonal);

		m_Rotation = (float)(std::atan2f(diagonal.y, diagonal.x) * 180.0f) / (float)M_PI;

		updateText();
	}
	else
	{
		m_Conquered = true;
	}
}

void UIText::calculatePosition(Vector2DInt realmPositions, unsigned int numberOfRealmPositions)
{
	Vector2DInt midRealmPosition = realmPositions / numberOfRealmPositions;
	Vector2D midRealmPositionScreen = Map::get().convertToScreen(midRealmPosition);
	m_PositionX = midRealmPositionScreen.x;
	m_PositionY = midRealmPositionScreen.y;
}

Vector2D UIText::calculateDiagonal(Vector2D topLeftMostPositionScreen, Vector2D bottomRightMostPositionScreen,
	Vector2D bottomLeftMostPositionScreen, Vector2D topRightMostPositionScreen)
{
	Vector2D topLeftToBottomRightDiagonal = bottomRightMostPositionScreen - topLeftMostPositionScreen;
	Vector2D bottomLeftToTopRightDiagonal = topRightMostPositionScreen - bottomLeftMostPositionScreen;
	float topLeftbottomRightLength = topLeftToBottomRightDiagonal.getLength();
	float bottomLeftToTopRightLength = bottomLeftToTopRightDiagonal.getLength();
	if (topLeftbottomRightLength >= bottomLeftToTopRightLength)
	{
		return topLeftToBottomRightDiagonal;
	}
	else
	{
		return bottomLeftToTopRightDiagonal;
	}
}

void UIText::calculateSize(Vector2D diagonal)
{
	float diagonalLength = diagonal.getLength();
	if ((unsigned int)(diagonalLength * 0.1f) >= m_MinCharacterSize
		&& (unsigned int)(diagonalLength * 0.1f) <= m_MaxCharacterSize)
	{
		m_CharacterSize = (unsigned int)(diagonalLength * 0.1f);
		m_OutlineThickness = diagonalLength * 0.001f;
	}
	else if ((unsigned int)(diagonalLength * 0.1f) < m_MinCharacterSize)
	{
		m_CharacterSize = m_MinCharacterSize;
		m_OutlineThickness = m_MinOutlineThickness;
	}
	else
	{
		m_CharacterSize = m_MaxCharacterSize;
		m_OutlineThickness = m_MaxOutlineThickness;
	}
}
