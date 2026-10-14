void RegionWindow::clickOnMap()
{
	if (InputHandler::getLeftMouseReleased() && !InputHandler::getPlayerUnitSelected()
		&& !InputHandler::getCharacterWindowOpen())
	{
		Vector2D mousePosition = InputHandler::getUIMousePosition();
		if (!m_WindowShape.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			InputHandler::setLeftMouseReleased(false);
			Vector2DInt mouseMapPosition = InputHandler::getMouseMapPosition();
			Map& map = Map::get();
			if (map.mapSquareDataContainsKey(mouseMapPosition))
			{
				size_t regionID = 0;

				for (auto& squareData : map.m_MapSquareData)
				{
					if (squareData.m_Position == mouseMapPosition)
					{
						regionID = squareData.m_RegionID;
					}
				}
				m_CurrentRegionID = regionID;
				m_CurrentMapRegion = &map.getRegionById(m_CurrentRegionID);
				checkIfPlayerRegion();
				updateInfo();
				m_Open = true;
			}
			else
			{
				closeWindow();
			}
		}
		else
		{
			m_Open = false;
		}
	}
	else if (InputHandler::getRightMouseReleased() && !InputHandler::getPlayerUnitSelected() && m_Visible)
	{
		Vector2D mousePosition = InputHandler::getUIMousePosition();
		if (!m_WindowShape.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			InputHandler::setRightMouseReleased(false);
			closeWindow();
		}
	}
}

void RegionWindow::updateInfo()
{
	if (m_CurrentMapRegion != nullptr)
	{
		m_PlayerCharacter = &CharacterManager::get().getPlayerCharacter();

		Character& character = CharacterManager::get().getCharacter(m_CurrentMapRegion->m_OwnerID);

		m_OwnerColor = character.m_RegionColor;

		m_WindowShape.setOutlineColor(m_OwnerColor);

		const char* portraitPath = character.m_PortraitPath.c_str();
		bool textureLoaded = false;
		while (!textureLoaded)
		{
			m_CharacterTexture = AssetHandler::get().getTextureAtPath(portraitPath);
			if (m_CharacterTexture.getSize().x != 0)
			{
				textureLoaded = true;
			}
		}
		setSprite(m_CharacterSprite, m_CharacterTexture, m_CharacterPosition, m_SpriteSize);

		std::stringstream stream;
		stream << m_CurrentMapRegion->m_RegionTax;
		m_RegionTaxText.setString(stream.str());
		m_RegionTaxText.setFillColor(m_OwnerColor);
		m_RegionTaxInfoText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		stream << m_CurrentMapRegion->m_ManPower;
		m_RegionManpowerText.setString(stream.str());
		m_RegionManpowerText.setFillColor(m_OwnerColor);
		m_RegionManpowerInfoText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		stream << m_CurrentMapRegion->m_RegionName;
		m_RegionNameText.setString(stream.str());
		m_RegionNameText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		stream << character.m_KingdomName;
		m_KingdomNameText.setString(stream.str());
		m_KingdomNameText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		m_RaiseArmyShape.setOutlineColor(m_OwnerColor);
		if (UnitManager::get().getUnitOfCharacter(character.m_CharacterID).m_Raised)
		{
			m_RaiseArmyShape.setFillColor(m_OwnerColor);
			m_RaiseArmySprite.setColor(m_OwnerColor);
		}
		else
		{
			m_RaiseArmyShape.setFillColor(sf::Color::Transparent);
			m_RaiseArmySprite.setColor(sf::Color::White);
		}

		for (unsigned int index = 0; index < NUMBER_OF_BUILDING_SLOTS; index++)
		{
			m_BuildingSlotShapes[index].setOutlineColor(m_OwnerColor);
			if (m_CurrentMapRegion->m_BuildingSlots[index].m_Finished)
			{
				m_BuildingSlotSprites[index].setColor(m_OwnerColor);
			}
			else
			{
				m_BuildingSlotShapes[index].setFillColor(sf::Color::Transparent);
			}
		}
	}
}
