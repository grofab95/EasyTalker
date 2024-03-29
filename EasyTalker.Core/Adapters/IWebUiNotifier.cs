﻿using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.File;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;

namespace EasyTalker.Core.Adapters;

public interface IWebUiNotifier
{
    void ConversationCreated(ConversationDto conversation);
    void ConversationUpdated(ConversationDto conversation);
    void MessageCreated(MessageDto message);
    void UserRegistered(UserDto user);
    void FileUploaded(FileDto file);
}