
void CharacterManager::calculateDeathChance(Character& character)
{
	if (!character.m_Dead)
	{
		unsigned int age = Time::m_GameDate.getAge(character.m_Birthday);
		float dividend = (float)((age - CharacterConstants::m_DeadlyAge) * (age - CharacterConstants::m_DeadlyAge));
		float divisor = (float)(CharacterConstants::m_AgeMax * CharacterConstants::m_AgeMax);
		float dieChance = (dividend / divisor) * m_MortalityRate;
		age < CharacterConstants::m_DeadlyAge ? dieChance *= CharacterConstants::m_YoungAgeModifier : dieChance;
		chancePerPercent(dieChance) ? killCharacter(character.m_CharacterID : false;
	}
}
void CharacterManager::handleInheritance(Character& character)
{
	std::vector<CharacterID> aliveChildren = getAliveChildren(character.m_CharacterID);

	if (character.m_IsPlayerControlled && aliveChildren.size() <= 0)
	{
		Player::get().loseGame(LoseCause::No_Heir);
	}

	if (aliveChildren.size() <= 0)
	{
		for (unsigned int ownedRegionID : character.m_OwnedRegionIDs)
		{
			CharacterID newCharacterID = m_CharacterCreator.createRandomUnlandedCharacter(m_CharacterPool,
				CharacterConstants::m_MinAgeNewUnlandedChar,
				CharacterConstants::m_MaxAgeNewUnlandedChar);
			Character& newCharacter = getCharacter(newCharacterID);
			newCharacter.m_RegionColor = sf::Color(
				(sf::Uint8)std::rand(),
				(sf::Uint8)std::rand(),
				(sf::Uint8)std::rand());
			addRegion(newCharacter.m_CharacterID, ownedRegionID);
			updateTitleAndUIText(newCharacter);
			UIManager::get().AdjustOwnership(newCharacter.m_CharacterID, character.m_CharacterID, ownedRegionID);
		}
		return;
	}

	if (character.m_OwnedRegionIDs.size() > 0)
	{
		size_t regionsByChild = character.m_OwnedRegionIDs.size() / aliveChildren.size();
		size_t giveOneMoreToChildren = character.m_OwnedRegionIDs.size() % aliveChildren.size();
		float goldByChild = character.m_CurrentGold / aliveChildren.size();

		if (aliveChildren.size() > character.m_OwnedRegionIDs.size())
		{
			regionsByChild = 1;
			giveOneMoreToChildren = 0;
		}

		size_t currentChild = 0;
		size_t currentChildInheritedRegions = 0;
		Character& currentChildCharacter = getCharacter(aliveChildren[currentChild]);
		currentChildCharacter.m_CurrentGold += goldByChild;
		bool childGotOneMore = false;
		if (currentChildCharacter.m_RegionColor == sf::Color::Black)
		{
			currentChildCharacter.m_RegionColor = getRandomColor();
		}

		if (!currentChildCharacter.m_IsPlayerControlled)
		{
			currentChildCharacter.m_IsPlayerControlled = character.m_IsPlayerControlled;
		}
		if (currentChildCharacter.m_IsPlayerControlled && m_PlayerCharacterID != currentChildCharacter.m_CharacterID)
		{
			m_PlayerCharacterID = currentChildCharacter.m_CharacterID;
			m_PlayerCharacter = &currentChildCharacter;
			AIManager::get().deactivateAI(currentChildCharacter.m_CharacterID);
			UIManager::get().m_DateBar->updateOwnerColor(currentChildCharacter.m_RegionColor);
			UIManager::get().m_StatBar->updateOwnerColor(currentChildCharacter.m_RegionColor);
			UIManager::get().m_MiniMap->setPlayerColor(currentChildCharacter.m_RegionColor);
		}

		for (size_t i = 0; i < character.m_OwnedRegionIDs.size(); ++i)
		{
			addRegion(currentChildCharacter.m_CharacterID, character.m_OwnedRegionIDs[i]);
			currentChildInheritedRegions++;

			if (currentChildInheritedRegions >= regionsByChild && i < character.m_OwnedRegionIDs.size() - 1)
			{
				if (giveOneMoreToChildren <= currentChild || childGotOneMore)
				{
					updateTitleAndUIText(currentChildCharacter);
					UIManager::get().AdjustOwnerships(currentChildCharacter.m_CharacterID, character.m_CharacterID,
						currentChildCharacter.m_OwnedRegionIDs);

					++currentChild;
					currentChildCharacter = getCharacter(aliveChildren[currentChild]);
					currentChildInheritedRegions = 0;
					currentChildCharacter.m_CurrentGold += goldByChild;
					childGotOneMore = false;

					if (currentChildCharacter.m_RegionColor == sf::Color::Black)
					{
						currentChildCharacter.m_RegionColor = getRandomColor();
					}
				}
				else
				{
					childGotOneMore = true;
				}
			}
		}
		
		updateTitleAndUIText(currentChildCharacter);
		UIManager::get().AdjustOwnerships(currentChildCharacter.m_CharacterID, character.m_CharacterID,
			currentChildCharacter.m_OwnedRegionIDs);
	}

	if (character.m_UnitEntity != INVALID_UNIT_ID)
	{
		UnitManager::get().dismissUnit(character.m_UnitEntity);
		character.m_UnitEntity = INVALID_UNIT_ID;
	}
}
