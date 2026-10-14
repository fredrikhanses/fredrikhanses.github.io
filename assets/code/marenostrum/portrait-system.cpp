
void CharacterCreator::setRandomPortraitPath(CharacterID& characterID)
{
	Character& character = CharacterManager::get().getCharacter(characterID);
	unsigned int age = Time::m_GameDate.getAge(character.m_Birthday);
	if (character.m_Gender == Gender::Male)
	{
		if (age < CharacterConstants::m_AgeOfConsent)
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfChildMalePortraits,
				CharacterConstants::m_ChildMalePortraitPath);
		}
		else if (age < CharacterConstants::m_AgeOfConsent * 2)
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfYoungMalePortraits,
				CharacterConstants::m_YoungMalePortraitPath);
		}
		else if (age < CharacterConstants::m_DeadlyAge)
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfAdultMalePortraits,
				CharacterConstants::m_AdultMalePortraitPath);
		}
		else
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfOldMalePortraits,
				CharacterConstants::m_OldMalePortraitPath);
		}
	}
	else
	{
		if (age < CharacterConstants::m_AgeOfConsent)
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfChildFemalePortraits,
				CharacterConstants::m_ChildFemalePortraitPath);
		}																	
		else if (age < CharacterConstants::m_AgeOfConsent * 2)				
		{																	
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfYoungFemalePortraits,
				CharacterConstants::m_YoungFemalePortraitPath);
		}
		else if (age < CharacterConstants::m_DeadlyAge)
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfAdultFemalePortraits,
				CharacterConstants::m_AdultFemalePortraitPath);
		}
		else
		{
			setRandomPortraitPath(character, CharacterConstants::m_NumberOfOldFemalePortraits,
				CharacterConstants::m_OldFemalePortraitPath);
		}
	}
}

void CharacterCreator::setRandomPortraitPath(Character& character, const unsigned int numberOfPortraits,
	const char* portraitPath)
{
	character.m_PortraitIndex = rand() * numberOfPortraits / RAND_MAX;
	std::stringstream stream;
	stream << portraitPath << character.m_PortraitIndex + 1 << CharacterConstants::m_PortraitPathEnding;
	character.m_PortraitPath = stream.str();
	stream.str(std::string());
	stream.clear();
}
