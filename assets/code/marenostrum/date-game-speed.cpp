void DateBar::clickButton()
{
	if (InputHandler::getLeftMouseReleased())
	{
		Vector2D mousePosition = InputHandler::getUIMousePosition();
		for (unsigned int index = 0; index < m_NumberOfButtons; index++)
		{
			if (m_ButtonShapes[index].getGlobalBounds().contains(mousePosition.x, mousePosition.y))
			{
				InputHandler::setLeftMouseReleased(false);
				switch (index)
				{
					case 0:
					{	
						Time::gamePaused() ? Time::unpauseGame() : Time::pauseGame();
						break;
					}
					case 1:
					{
						Time::decreaseGameSpeed();
						break;
					}
					case 2:
					{
						Time::increaseGameSpeed();
						break;
					}
					default:
					{
						break;
					}
				}
			}
			if (m_Paused != Time::gamePaused() || m_CurrentSpeedLevel != (unsigned int)Time::m_CurrentSpeedLevel)
			{
				break;
			}
		}
		for (unsigned int index = 0; index < m_NumberOfSpeeds; index++)
		{
			if (m_SpeedShapes[index].getGlobalBounds().contains(mousePosition.x, mousePosition.y))
			{
				InputHandler::setLeftMouseReleased(false);
				Time::setGameSpeed(index + 1);
				break;
			}
		}
		if (m_WindowShape.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			InputHandler::setLeftMouseReleased(false);
		}
	}
	if (m_Paused != Time::gamePaused() || m_CurrentSpeedLevel != (unsigned int)Time::m_CurrentSpeedLevel)
	{
		updateSpeedShapes();
	}
}

void DateBar::updateSpeedShapes()
{
	m_CurrentSpeedLevel = Time::m_CurrentSpeedLevel;
	for (unsigned int index = 0; index < m_NumberOfSpeeds; index++)
	{
		if (index < m_CurrentSpeedLevel && Time::gamePaused())
		{
			m_SpeedShapes[index].setFillColor(m_PauseSpeedColor);
			if (m_Paused != Time::gamePaused())
			{
				setSprite(m_ButtonSprites.front(), m_ButtonTextures.back(), m_ButtonSprites.front().getPosition());
			}
		}
		else if (index < m_CurrentSpeedLevel)
		{
			m_SpeedShapes[index].setFillColor(m_PlaySpeedColor);
			if (m_Paused != Time::gamePaused())
			{
				setSprite(m_ButtonSprites.front(), m_ButtonTextures.front(), m_ButtonSprites.front().getPosition());
			}
		}
		else
		{
			m_SpeedShapes[index].setFillColor(m_TransparentColor);
		}
	}
	m_Paused = Time::gamePaused();
}