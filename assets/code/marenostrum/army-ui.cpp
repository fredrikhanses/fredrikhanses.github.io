void Player::clickDrag()
{
	if (m_MouseDownLastFrame && !m_Draging)
	{
		m_Draging = true;
		m_DragWindow.setPosition(m_MousePosition.x, m_MousePosition.y);
	}

	if (m_Draging)
	{
		Vector2D mousePosition = InputHandler::getMousePosition();
		Vector2D distance = mousePosition - m_MousePosition;
		m_DragWindow.setSize(sf::Vector2f(distance.x, distance.y));
		m_DragWindow.setOutlineThickness(m_DragThickness * InputHandler::m_TotalZoom);

		Vector2D direction = distance.normalized();
		m_DragDirection.x = direction.x;
		m_DragDirection.y = direction.y;
	}

	if (InputHandler::getLeftMouseClicked())
	{
		m_MouseDownLastFrame = true;
		m_MousePosition = InputHandler::getMousePosition();
	}
	else if (InputHandler::getLeftMouseReleased() && m_Draging)
	{
		m_MouseDownLastFrame = false;
		m_Draging = false;
	}
}

void Player::tryToSelectUnit()
{
	if (InputHandler::getLeftMouseClicked() && !m_Draging)
	{
		Vector2D mousePosition = InputHandler::getMousePosition();
		CharacterID playerCharacterID = CharacterManager::get().getPlayerCharacterID();
		Unit& unit = UnitManager::get().getUnitOfCharacter(playerCharacterID);
		if (unit.m_Sprite.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			selectUnit(unit.m_UnitID);
		}
		else
		{
			deselectUnit();
		}
	}

	if (m_Draging && InputHandler::getMouseMoved())
	{
		CharacterID playerCharacterID = CharacterManager::get().getPlayerCharacterID();
		Unit& unit = UnitManager::get().getUnitOfCharacter(playerCharacterID);
		Vector2D unitPosition = { unit.m_Position.x + unit.m_HighlightShapeSize.x * 0.5f,
			unit.m_Position.y + unit.m_HighlightShapeSize.y * 0.5f };
		if (m_DragWindow.getGlobalBounds().contains(unitPosition.x, unitPosition.y))
		{
			selectUnit(unit.m_UnitID);
		}
		else
		{
			deselectUnit();
		}
	}
}

void Player::selectUnit(UnitID id)
{
	Unit& unit = UnitManager::get().getUnitWithId(id);
	unit.m_Selected = true;
	InputHandler::setPlayerUnitSelected(true);
	m_SelectedUnitID = id;
}

void Player::deselectUnit()
{
	if (m_SelectedUnitID == INVALID_UNIT_ID)
	{
		return;
	}

	Unit& unit = UnitManager::get().getUnitWithId(m_SelectedUnitID);
	unit.m_Selected = false;
	InputHandler::setPlayerUnitSelected(false);
	m_SelectedUnitID = INVALID_UNIT_ID;
}
void UnitManager::showLinePath(Unit& unit)
{
	m_UnitLinePath.clear();
	for each (Vector2DInt position in unit.m_CurrentPath)
	{
		Vector2D screenPosition = Map::convertToScreen(position);
		m_UnitLinePath.append(sf::Vertex(
			{ screenPosition.x + m_SpriteSize * 0.5f, screenPosition.y + m_SpriteSize * 0.5f }));
	}
}
