void StatBar::updateStats()
{
	Character& character = CharacterManager::get().getPlayerCharacter();

	std::stringstream stream;
	stream << std::fixed << std::setprecision(1) << character.m_CurrentGold;
	m_CurrentGoldText.setString(stream.str());
	stream.str(std::string());
	stream.clear();

	float income = character.m_Income;
	if (income >= 0.0f)
	{
		stream << m_PositiveSign << std::fixed << std::setprecision(1) << character.m_Income;
	}
	else
	{
		stream << std::fixed << std::setprecision(1) << character.m_Income;
	}
	m_CurrentIncomeText.setString(stream.str());
	stream.str(std::string());
	stream.clear();

	stream << character.m_MaxArmySize;
	m_MaxArmyText.setString(stream.str());
	stream.str(std::string());
	stream.clear();

	stream << UnitManager::get().getUnitOfCharacter(character.m_CharacterID).m_RepresentedForce;
	m_CurrentMaxArmyText.setString(stream.str());
	stream.str(std::string());
	stream.clear();
}