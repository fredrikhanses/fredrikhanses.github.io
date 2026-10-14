void Player::moveUnit()
{
	if (InputHandler::getRightMouseReleased() && m_SelectedUnitID != INVALID_UNIT_ID)
	{
		InputHandler::setRightMouseReleased(false);
		Vector2DInt mousePosition = InputHandler::getMouseMapPosition();
		Vector2DInt unitPosition = Map::get().convertToMap(
			UnitManager::get().getUnitWithId(m_SelectedUnitID).m_Position);
		UnitManager::get().giveUnitPath(m_SelectedUnitID, Pathfinding::get().findPath(unitPosition, mousePosition));
	}
}

void UnitManager::moveUnit(Unit& unit)
{
	if (!unit.m_Position.nearlyEqual(unit.m_Target, m_MoveTolerance))
	{
		unit.m_Position += unit.m_Direction * unit.m_Speed * Time::deltaTime();
	}
	else
	{
		unit.m_LastPosition = unit.m_Target;
		unit.m_Position = unit.m_Target;
		if (unit.m_CurrentPath.size() > 0)
		{
			Vector2D nextPosition = Map::convertToScreen(unit.m_CurrentPath.front());
			unit.m_Target = nextPosition;
			unit.m_Direction = (nextPosition - unit.m_Position).normalized();
			unit.m_CurrentPath.erase(unit.m_CurrentPath.begin());
		}
		else
		{
			unit.m_Moving = false;
		}
	}
}
