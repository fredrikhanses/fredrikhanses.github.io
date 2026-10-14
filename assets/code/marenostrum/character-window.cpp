void CharacterWindow::clickOnMap()
{
	if (InputHandler::getRightMouseClicked() && !InputHandler::getPlayerUnitSelected())
	{
		Vector2D mousePosition = InputHandler::getUIMousePosition();
		if (!m_WindowShape.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			InputHandler::setRightMouseClicked(false);
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
				m_CurrentCharacterID = map.getRegionById(m_CurrentRegionID).m_OwnerID;
				checkIfPlayerCharacter();
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
	else if (InputHandler::getLeftMouseClicked() && !InputHandler::getPlayerUnitSelected() && m_Visible)
	{
		Vector2D mousePosition = InputHandler::getUIMousePosition();
		if (!m_WindowShape.getGlobalBounds().contains(mousePosition.x, mousePosition.y))
		{
			InputHandler::setLeftMouseClicked(false);
			closeWindow();
		}
	}
}

void CharacterWindow::updateInfo()
{
	if (m_CurrentCharacterID != INVALID_CHARACTER_ID)
	{
		CharacterManager& characterManager = CharacterManager::get();
		m_PlayerCharacter = &characterManager.getPlayerCharacter();
		m_CurrentCharacter = &characterManager.getCharacter(m_CurrentCharacterID);
		m_FatherID = m_CurrentCharacter->m_Father;
		m_MotherID = m_CurrentCharacter->m_Mother;
		m_Dead = m_CurrentCharacter->m_Dead;
		m_Gender = m_CurrentCharacter->m_Gender;

		if (m_CurrentCharacter->m_CharacterTitle != Title::Unlanded)
		{
			m_OwnerColor = m_CurrentCharacter->m_RegionColor;
			m_RealmNameText.setString(m_CurrentCharacter->m_KingdomName);
			m_RealmNameText.setFillColor(m_OwnerColor);
		}
		else if (m_FatherID != INVALID_CHARACTER_ID)
		{
			Character& father = characterManager.getCharacter(m_FatherID);
			m_OwnerColor = father.m_RegionColor;
			m_RealmNameText.setString(father.m_KingdomName);
			m_RealmNameText.setFillColor(m_OwnerColor);
		}
		else
		{
			m_RealmNameText.setString(m_EmptyString);
			m_OwnerColor = m_CurrentCharacter->m_RegionColor;
		}

		m_WindowShape.setOutlineColor(m_OwnerColor);
		m_FamilyTreeText.setFillColor(m_OwnerColor);

		for (sf::Text& text : m_DiplomacyTexts)
		{
			text.setFillColor(m_OwnerColor);
		}

		clearInfo();
		updateParents();
		updateTraits();
		updateChildren();
		updateAllies();
		updateWars();
		updateTruces();

		Map& map = Map::get();
		unsigned int ownedRegionSize = m_CurrentCharacter->m_OwnedRegionIDs.size();
		for (unsigned int index = 0; index < ownedRegionSize; index++)
		{
			sf::RectangleShape regionShape;
			setShape(regionShape, m_TransparentColor, m_TransparentColor, 0.0f, 
				{ m_SpriteSize, m_SpriteSize }, 
				{ m_SizeX * 0.1f, m_SpriteSize * index + m_OutlineThickness * 1.5f * index + m_SpriteSize * 12.0f });

			MapRegion& mapRegion = map.getRegionById(m_CurrentCharacter->m_OwnedRegionIDs[index]);
			sf::Text regionNameText;
			setText(regionNameText, m_Font, m_CharacterSize, m_OwnerColor, 
				{ regionShape.getPosition().x, regionShape.getPosition().y + m_SpriteSize }, mapRegion.m_RegionName.c_str());

			m_OwnedRegionShapes.push_back(regionShape);
			m_OwnedRegionsTexts.push_back(regionNameText);
			m_OwnedRegionShields.push_back(&mapRegion.m_HeraldicShield);
		}

		std::stringstream stream;
		if (m_Gender == Gender::Male)
		{
			if (m_CurrentCharacter->m_CharacterTitle >= Title::Unlanded)
			{
				stream << m_CurrentCharacter->m_Name;
			}
			else
			{
				stream << m_MaleTitles[(unsigned int)m_CurrentCharacter->m_CharacterTitle] << m_CurrentCharacter->m_Name;
			}
		}
		else
		{
			if (m_CurrentCharacter->m_CharacterTitle >= Title::Unlanded)
			{
				stream << m_CurrentCharacter->m_Name;
			}
			else
			{
				stream << m_FemaleTitles[(unsigned int)m_CurrentCharacter->m_CharacterTitle] << m_CurrentCharacter->m_Name;
			}
		}
		m_CharacterNameText.setString(stream.str());
		m_CharacterNameText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		if (m_CurrentCharacterID != m_PreviousCharacterID)
		{
			m_PreviousCharacterID = m_CurrentCharacterID;
			const char* portraitPath = m_CurrentCharacter->m_PortraitPath.c_str();
			bool textureLoaded = false;
			while (!textureLoaded)
			{
				m_CurrentCharacterTexture = AssetHandler::get().getTextureAtPath(portraitPath);
				if (m_CurrentCharacterTexture.getSize().x != 0)
				{
					textureLoaded = true;
				}
			}
			setSprite(m_CharacterSprite, m_CurrentCharacterTexture, { m_SizeX * 0.1f, m_SpriteSize }, m_SpriteSize * 2);
		}

		Unit& unit = UnitManager::get().getUnitOfCharacter(m_CurrentCharacterID);
		stream << unit.m_RepresentedForce << m_Dash << m_CurrentCharacter->m_MaxArmySize;
		m_ArmyText.setString(stream.str());
		m_ArmyText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		stream << std::fixed << std::setprecision(1) << m_CurrentCharacter->m_CurrentGold;
		m_GoldText.setString(stream.str());
		m_GoldText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		if (!m_Dead)
		{
			stream << Time::m_GameDate.getAge(m_CurrentCharacter->m_Birthday);
		}
		else
		{
			stream << m_CurrentCharacter->m_AgeAtDeath;
		}
		m_CharacterAgeText.setString(stream.str());
		m_CharacterAgeText.setFillColor(m_OwnerColor);
		stream.str(std::string());
		stream.clear();

		if (m_CurrentCharacter->m_Dead)
		{
			if (m_Gender == Gender::Male)
			{
				setSprite(m_BirthSprite, m_MaleChildTexture, m_BirthPosition);
			}
			else
			{
				setSprite(m_BirthSprite, m_FemaleChildTexture, m_BirthPosition);
			}

			stream << Time::m_GameDate.getDateString(m_CurrentCharacter->m_Birthday);
			m_Birthday.setString(stream.str());
			m_Birthday.setFillColor(m_OwnerColor);
			stream.str(std::string());
			stream.clear();

			stream << Time::m_GameDate.getDateString(m_CurrentCharacter->m_DeathDate);
			m_DeathDate.setString(stream.str());
			m_DeathDate.setFillColor(m_OwnerColor);
			stream.str(std::string());
			stream.clear();
		}
	}
}
