
EventWindow::EventWindow(UIID ID, sf::Font font, CharacterID instigatorID, CharacterID subjectID, UIType type,
	float goldAmount, std::string buildingName, std::string regionName)
{
	m_OwnedUIWindow = ID;
	m_Font = font;
	m_InstigatorID = instigatorID;
	m_SubjectID = subjectID;
	m_MessageType = type;

	m_Window = Window::getWindow();

	Character& playerCharacter = CharacterManager::get().getPlayerCharacter();
	m_OwnerColor = playerCharacter.m_RegionColor;

	Character& instigator = CharacterManager::get().getCharacter(m_InstigatorID);
	m_InstigatorColor = instigator.m_RegionColor;

	Character& subject = CharacterManager::get().getCharacter(m_SubjectID);
	m_SubjectColor = subject.m_RegionColor;

	AssetHandler& assetHandler = AssetHandler::get();

	m_PositionX = m_Window->getSize().x * 0.5f - m_OutlineThickness - m_SizeX * 0.5f;
	m_PositionY = m_Window->getSize().y * 0.5f - m_OutlineThickness - m_SizeY * 0.5f;

	const char* subjectPortraitPath = subject.m_PortraitPath.c_str();
	bool textureLoaded = false;
	while (!textureLoaded)
	{
		m_SubjectTexture = AssetHandler::get().getTextureAtPath(subjectPortraitPath);
		if (m_SubjectTexture.getSize().x != 0)
		{
			textureLoaded = true;
		}
	}
	setSprite(m_SubjectSprite, m_SubjectTexture,
		{ m_PositionX + m_SizeX - m_SpriteSize * 1.5f, m_PositionY + m_SpriteSize * 0.5f },
		m_SpriteSize);

	const char* instigatorPortraitPath = instigator.m_PortraitPath.c_str();
	textureLoaded = false;
	while (!textureLoaded)
	{
		m_InstigatorTexture = AssetHandler::get().getTextureAtPath(instigatorPortraitPath);
		if (m_InstigatorTexture.getSize().x != 0)
		{
			textureLoaded = true;
		}
	}
	setSprite(m_InstigatorSprite, m_InstigatorTexture,
		{ m_PositionX + m_SpriteSize * 0.5f, m_PositionY + m_SpriteSize * 0.5f },
		m_SpriteSize);

	std::stringstream stream;
	stream << instigator.m_Name;
	switch (m_MessageType)
	{
		case UIType::MarriageRequest:
		case UIType::MarriageAccepted:
		case UIType::MarriageDeclined:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Married.png");
			if (m_MessageType == UIType::MarriageRequest)
			{
				stream << "\nwants to marry you.\n\n               Do you accept?";
			}
			else if (m_MessageType == UIType::MarriageAccepted)
			{
				stream << "\naccepted your marriage proposal!";
			}
			else
			{
				stream << "\ndeclined your marriage proposal!";
			}
			break;
		}
		case UIType::AllianceRequest:
		case UIType::AllianceAccepted:
		case UIType::AllianceDeclined:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Alliance.png");
			if (m_MessageType == UIType::AllianceRequest)
			{
				stream << "\nwants to ally you.\n\n               Do you accept?";
			}
			else if (m_MessageType == UIType::AllianceAccepted)
			{
				stream << "\naccepted your alliance proposal!";
			}
			else
			{
				stream << "\ndeclined your alliance proposal!";
			}
			break;
		}
		case UIType::AllianceBroken:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Alliance.png");
			stream.str(std::string());
			stream.clear();
			stream << "Your alliance with\n";
			writeTitle(subject, stream);
			stream << subject.m_Name << "\nis broken.";
			break;
		}
		case UIType::CallToArmsRequest:
		case UIType::CallToArmsAccepted:
		case UIType::CallToArmsRejected:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Alliance.png");
			if (m_MessageType == UIType::CallToArmsRequest)
			{
				stream << "\nis calling you to arms.\n\n               Do you honor it?";
			}
			else if (m_MessageType == UIType::CallToArmsAccepted)
			{
				stream << "\naccepted your call to arms!";
			}
			else
			{
				stream << "\ndeclined your call to arms!";
			}
			break;
		}
		case UIType::PeaceRequestEnforce:
		case UIType::PeaceRequestWhite:
		case UIType::PeaceRequestSurrender:
		case UIType::PeaceAccepted:
		case UIType::PeaceDeclined:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Peace.png");
			if (m_MessageType == UIType::PeaceRequestEnforce)
			{
				stream << "\nis enforcing their demands.\nYou have no choice but to accept.";
			}
			else if (m_MessageType == UIType::PeaceRequestWhite)
			{
				stream << "\nwants a white peace.\n\n               Do you accept?";
			}
			else if (m_MessageType == UIType::PeaceRequestSurrender)
			{
				stream << "\nsurrenders to you.\n\n               Do you accept?";
			}
			else if (m_MessageType == UIType::PeaceAccepted)
			{
				stream << "\naccepted your peace offer!";
			}
			else
			{
				stream << "\ndeclined your peace offer!";
			}
			break;
		}
		case UIType::TruceMessage:
		case UIType::TriedToDeclareWarOnAlly:
		case UIType::WarDeclaration:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/War.png");
			if (m_MessageType == UIType::WarDeclaration)
			{
				if (instigatorID == playerCharacter.m_CharacterID)
				{
					stream << "\ndeclared war on\n";
					writeTitle(subject, stream);
					stream << subject.m_Name << "!";
				}
				else
				{
					stream << "\ndeclared war on you!";
				}
			}
			else if (m_MessageType == UIType::TriedToDeclareWarOnAlly)
			{
				stream.str(std::string());
				stream.clear();
				stream << "You can't declare war on an ally!";
			}
			else
			{
				stream.str(std::string());
				stream.clear();
				stream << "You can't declare war on\n";
				writeTitle(subject, stream);
				stream << subject.m_Name << "\nYou have a truce!";
			}
			break;
		}
		case UIType::BuildingMessage:
		case UIType::CannotAffordMessage:
		case UIType::Gift:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Coins.png");
			if (m_MessageType == UIType::CannotAffordMessage)
			{
				stream.str(std::string());
				stream.clear();
				stream << "You can't afford the cost\nof " << std::fixed << std::setprecision(1) << goldAmount
					<< " gold to do this.";
			}
			else if(m_MessageType == UIType::Gift)
			{
				stream << "\nsent you a gift of " << std::fixed << std::setprecision(1) << goldAmount << " gold!";
			}
			else
			{
				stream.str(std::string());
				stream.clear();
				stream << "Started construction of a " << buildingName << "\nin " << regionName
					<< " for a cost of\n" << std::fixed << std::setprecision(1) << goldAmount << " gold.";
			}
			break;
		}
		case UIType::Pregnant:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Pregnant.png");
			stream << "\njust got pregnant!";
			break;
		}
		case UIType::ChildBirth:
		{
			if (instigator.m_Children.size() > 0)
			{
				Character& child = CharacterManager::get().getCharacter(instigator.m_Children.back());
				if (child.m_Gender == Gender::Male)
				{
					m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/BabyMale.png");
					stream << "\njust gave birth to a healthy baby boy!" << "\nHis name is " << child.m_Name << ".";
				}
				else
				{
					m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/BabyFemale.png");
					stream << "\njust gave birth to a healthy baby girl!" << "\nHer name is " << child.m_Name << ".";
				}
			}
			break;
		}
		case UIType::AssassinationSuccess:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Assassinate.png");
			stream.str(std::string());
			stream.clear();
			writeTitle(subject, stream);
			stream << subject.m_Name << "\ndied under suspicious circumstances.";
			break;
		}
		case UIType::AssassinationFailure:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Assassinate.png");
			stream.str(std::string());
			stream.clear();
			writeTitle(subject, stream);
			stream << subject.m_Name << "\nevaded an assassination!";
			break;
		}
		case UIType::Death:
		{
			m_MessageTypeTexture = assetHandler.getTextureAtPath("Assets/Graphics/Dead.png");
			stream << "\nof the " << instigator.m_KingdomName << "\njust died!";
			break;
		}
		default:
		{
			break;
		}
	}
	setSprite(m_MessageTypeSprite, m_MessageTypeTexture,
		{ m_PositionX + m_SizeX * 0.5f - m_SpriteSize * 0.25f, m_PositionY + m_SpriteSize * 0.75f },
		(unsigned int)(m_SpriteSize * 0.5f));
	setText(m_MessageText, m_Font, m_CharacterSize, m_OwnerColor,
		{ m_PositionX + m_SpriteSize * 0.5f, m_PositionY + m_SpriteSize * 1.6f },
		stream.str().c_str());
	stream.str(std::string());
	stream.clear();

	setShape(m_WindowShape, m_FillColor, m_OwnerColor, m_OutlineThickness,
		{ m_SizeX, m_SizeY },
		{ m_PositionX, m_PositionY });

	setShape(m_InstigatorShape, m_TransparentColor, m_InstigatorColor, m_OutlineThickness * 0.5f,
		{ m_SpriteSize, m_SpriteSize },
		m_InstigatorSprite.getPosition());

	setShape(m_SubjectShape, m_TransparentColor, m_SubjectColor, m_OutlineThickness * 0.5f,
		{ m_SpriteSize, m_SpriteSize },
		m_SubjectSprite.getPosition());

	if (m_MessageType == UIType::MarriageRequest || m_MessageType == UIType::AllianceRequest 
		|| m_MessageType == UIType::PeaceRequestEnforce || m_MessageType == UIType::PeaceRequestWhite
		|| m_MessageType == UIType::PeaceRequestSurrender || m_MessageType == UIType::CallToArmsRequest)
	{
		setShape(m_AgreeShape, m_TransparentColor, m_AgreeColor, m_OutlineThickness * 0.5f, 
			{ m_SpriteSize * 1.25f, m_SpriteSize * 0.5f }, 
			{ m_PositionX + m_SizeX - m_SpriteSize * 1.75f, m_PositionY + m_SizeY - m_SpriteSize });
		setText(m_AgreeText, m_Font, m_CharacterSize, m_AgreeColor, m_AgreeShape.getPosition(), m_AgreeString);
	}

	if (m_MessageType != UIType::PeaceRequestEnforce)
	{
		setShape(m_DismissShape, m_TransparentColor, m_DismissColor, m_OutlineThickness * 0.5f, 
			{ m_SpriteSize * 1.25f, m_SpriteSize * 0.5f }, 
			{ m_PositionX + m_SpriteSize * 0.5f, m_PositionY + m_SizeY - m_SpriteSize });
		setText(m_DismissText, m_Font, m_CharacterSize, m_DismissColor, m_DismissShape.getPosition(), m_DismissString);
	}
	Time::pauseGame();
}
