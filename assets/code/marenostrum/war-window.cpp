void WarIcon::updateInfo()
{
	m_WarHandle = DiplomacyManager::get().getWarHandleAgainst(m_AttackerID, m_DefenderID);
	if (m_WarHandle != -1)
	{
		DiplomacyManager& diplomacyManager = DiplomacyManager::get();
		War& war = *diplomacyManager.getWar(m_WarHandle);
		MapRegion& wargoalRegion = Map::get().getRegionById(war.m_WargoalRegion);
		m_HeraldicShield = &wargoalRegion.m_HeraldicShield;

		std::stringstream stream;
		CharacterID playerCharacterID = CharacterManager::get().getPlayerCharacterID();
		m_PlayerAttacker = diplomacyManager.isAttacker(m_WarHandle, playerCharacterID);
		if (m_PlayerAttacker)
		{
			int warscore = diplomacyManager.getWarscore(m_WarHandle, m_AttackerID);
			setWarscore(warscore, stream);
		}
		else
		{
			int warscore = diplomacyManager.getWarscore(m_WarHandle, m_DefenderID);
			setWarscore(warscore, stream);
		}
		stream << m_PercentSign;
		m_WarscoreText.setString(stream.str());
		stream.str(std::string());
		stream.clear();

		m_WarscoreText.setOrigin(m_WarscoreText.getLocalBounds().width * 0.5f,
			m_WarscoreText.getLocalBounds().height * 0.75f);
	}
	else if (m_DaySubscriptionHandle != -1)
	{
		deactivate();
	}
}

void WarWindow::updateInfo()
{
	m_War = DiplomacyManager::get().getWarAgainst(m_AttackerCharacterIDs.front(), m_DefenderCharacterIDs.front());
	if (m_War != nullptr)
	{
		Character& attackerMain = CharacterManager::get().getCharacter(m_AttackerCharacterIDs.front());

		std::stringstream stream;
		stream << attackerMain.m_KingdomName << m_CasusBelli
			<< Map::get().getRegionById(m_War->m_WargoalRegion).m_RegionName;
		m_WarNameText.setString(stream.str());
		stream.str(std::string());
		stream.clear();

		stream << m_WarScore;
		m_WarscoreText.setString(stream.str());
		stream.str(std::string());
		stream.clear();

		int warscore = 0;
		CharacterID playerCharacterID = CharacterManager::get().getPlayerCharacterID();
		if (m_DefenderCharacterIDs.front() == playerCharacterID)
		{
			setWarscore(m_DefenderCharacterIDs.front(), stream, warscore);
			m_WarscoreProgressShape.setSize({
				m_WarscoreShape.getSize().x * 0.5f + m_WarscoreShape.getSize().x * 0.005f * warscore,
				m_WarscoreProgressShape.getSize().y });
		}
		else if (m_AttackerCharacterIDs.front() == playerCharacterID)
		{
			setWarscore(m_AttackerCharacterIDs.front(), stream, warscore);
			m_WarscoreProgressShape.setSize({
				m_WarscoreShape.getSize().x * 0.5f + m_WarscoreShape.getSize().x * 0.005f * warscore,
				m_WarscoreProgressShape.getSize().y });
		}
		else
		{
			setWarscore(m_AttackerCharacterIDs.front(), stream, warscore);
			m_WarscoreProgressShape.setSize({
				m_WarscoreShape.getSize().x * 0.5f + m_WarscoreShape.getSize().x * 0.005f * -warscore,
				m_WarscoreProgressShape.getSize().y });
		}
		stream << m_PercentSign;
		m_WarscoreAmountText.setString(stream.str());
		stream.str(std::string());
		stream.clear();

		int yearDiff = Time::m_GameDate.m_Date.m_Year - m_WarStartDate.m_Year;
		int monthDiff = Time::m_GameDate.m_Date.m_Month - m_WarStartDate.m_Month;
		if (yearDiff > 0)
		{
			stream << m_WarStarted << yearDiff;
			if (yearDiff == 1)
			{
				stream << m_YearAgo;
			}
			else
			{
				stream << m_YearsAgo;
			}
		}
		else if (monthDiff > 0)
		{
			stream << m_WarStarted << monthDiff;
			if (monthDiff == 1)
			{
				stream << m_MonthAgo;
			}
			else
			{
				stream << m_MonthsAgo;
			}
		}
		else
		{
			int dayDiff = Time::m_GameDate.m_Date.m_Day - m_WarStartDate.m_Day;
			stream << m_WarStarted << dayDiff;
			if (dayDiff == 1)
			{
				stream << m_DayAgo;
			}
			else
			{
				stream << m_DaysAgo;
			}
		}
		m_DurationText.setString(stream.str());
		stream.str(std::string());
		stream.clear();

		unsigned int totalArmy = 0;
		unsigned int totalRaisedArmy = 0;
		for (CharacterID attackerID : m_AttackerCharacterIDs)
		{
			Character& attacker = CharacterManager::get().getCharacter(attackerID);
			totalArmy += attacker.m_MaxArmySize;

			if (UnitManager::get().getUnitOfCharacter(attackerID).m_Raised)
			{
				totalRaisedArmy += UnitManager::get().getUnitOfCharacter(attackerID).m_RepresentedForce;
			}
		}
		stream << totalRaisedArmy << m_DashSign << totalArmy;
		m_AttackerArmyStrengthText.setString(stream.str());
		totalArmy = 0;
		totalRaisedArmy = 0;
		stream.str(std::string());
		stream.clear();

		for (CharacterID defenderID : m_DefenderCharacterIDs)
		{
			Character& defender = CharacterManager::get().getCharacter(defenderID);
			totalArmy += defender.m_MaxArmySize;

			if (UnitManager::get().getUnitOfCharacter(defenderID).m_Raised)
			{
				totalRaisedArmy += UnitManager::get().getUnitOfCharacter(defenderID).m_RepresentedForce;
			}
		}
		stream << totalRaisedArmy << m_DashSign << totalArmy;
		m_DefenderArmyStrengthText.setString(stream.str());
		totalArmy = 0;
		totalRaisedArmy = 0;
		stream.str(std::string());
		stream.clear();
	}
	else
	{
		closeWindow();
	}
}
