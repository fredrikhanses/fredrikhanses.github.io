
void FamilyTreeWindow::updateInfo()
{
	if (m_CurrentCharacterID != INVALID_CHARACTER_ID)
	{
		CharacterManager& characterManager = CharacterManager::get();
		m_CurrentCharacter = &characterManager.getCharacter(m_CurrentCharacterID);
		if (!m_CurrentCharacter->m_Dead)
		{
			m_OwnerColor = m_CurrentCharacter->m_RegionColor;
		}
		else
		{
			m_OwnerColor = m_CurrentCharacter->m_ColorAtDeath;
		}
		m_WindowShape.setOutlineColor(m_OwnerColor);
		m_WindowTitle.setFillColor(m_OwnerColor);

		displayFamily(findFamilyHead(m_CurrentCharacterID), 0, 0.0f);
	}
}

CharacterID& FamilyTreeWindow::findFamilyHead(CharacterID& characterID)
{
	Character& character = CharacterManager::get().getCharacter(characterID);
	if (character.m_Father == INVALID_CHARACTER_ID)
	{
		return characterID;
	}
	return findFamilyHead(character.m_Father);
}

void FamilyTreeWindow::displayFamily(CharacterID& familyHeadID, unsigned int depth, float width)
{
	if (familyHeadID == INVALID_CHARACTER_ID)
	{
		return;
	}
	Character& character = CharacterManager::get().getCharacter(familyHeadID);
	setFamilyMember(familyHeadID, depth, width);
	unsigned int numberOfSpouses = character.m_TotalSpouses.size();
	for (unsigned int index = 0; index < numberOfSpouses; index++)
	{
		setFamilyMember(character.m_TotalSpouses[index], depth, ++width);
	}
	displayFamily(character.m_OldestChild, depth + 1,
		character.m_Children.size() * 0.5f - character.m_Children.size() + 1);
	displayFamily(character.m_NextSibling, depth, width + 1);
}

void FamilyTreeWindow::setFamilyMember(CharacterID& characterID, unsigned int depth, float width)
{
	for (CharacterID familyID : m_CharacterIDs)
	{
		if (familyID == characterID)
		{
			return;
		}
	}
	m_CharacterIDs.push_back(characterID);
	m_ShowCharacterInfo.push_back(false);
	Character& character = CharacterManager::get().getCharacter(characterID);
	sf::Color characterColor;
	unsigned int characterTitle;
	sf::Vector2f characterPosition = { m_PositionX + m_SizeX * 0.5f - m_SpriteSize * 3.5f + 
        m_OutlineThickness + m_SpriteSize * 7 * width, 
        m_PositionY + m_OutlineThickness + m_SpriteSize * 2 + m_SpriteSize * 3 * depth };
	Date deathDate;
	if (!character.m_Dead)
	{
		characterColor = character.m_RegionColor;
		characterTitle = (unsigned int)character.m_CharacterTitle;
		m_CharacterDeadSprites.push_back(sf::Sprite());
	}
	else
	{
		characterColor = character.m_ColorAtDeath;
		characterTitle = (unsigned int)character.m_TitleAtDeath;
		deathDate = character.m_DeathDate;
		sf::Sprite deadSprite;
		setSprite(deadSprite, m_DeadTexture,
			{ characterPosition.x + m_OutlineThickness + m_SpriteSize, characterPosition.y });
		m_CharacterDeadSprites.push_back(deadSprite);
	}
	sf::RectangleShape characterShape;
	setShape(characterShape, m_TransparentColor, characterColor, m_OutlineThickness * 0.5f, 
        { m_SpriteSize, m_SpriteSize }, characterPosition);
	m_CharacterShapes.push_back(characterShape);

	const char* portraitPath = character.m_PortraitPath.c_str();
	bool textureLoaded = false;
	while (!textureLoaded)
	{
		sf::Texture texture = AssetHandler::get().getTextureAtPath(portraitPath);
		if (texture.getSize().x != 0)
		{
			textureLoaded = true;
			m_CharacterTextures.push_back(texture);
		}
	}
	sf::Sprite characterSprite;
	setSprite(characterSprite, m_CharacterTextures.back(), characterShape.getPosition(), m_SpriteSize);
	m_CharacterSprites.push_back(characterSprite);

	std::stringstream stream;
	stream << character.m_Name << "\n" << character.m_Birthday.m_Day << "-" 
    << character.m_Birthday.m_Month << "-" << character.m_Birthday.m_Year;
	if (character.m_Dead)
	{
		stream << " to " << character.m_DeathDate.m_Day << "-" 
        << character.m_DeathDate.m_Month << "-" << character.m_DeathDate.m_Year;
	}
	sf::Text characterText;
	setText(characterText, m_Font, m_CharacterSize, characterColor, 
        { characterShape.getPosition().x, characterShape.getPosition().y + m_SpriteSize}, stream.str().c_str());
	m_CharacterInfo.push_back(characterText);
}
