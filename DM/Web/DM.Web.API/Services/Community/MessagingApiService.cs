using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.Messaging.Reading;
using DM.Services.Core.Dto;
using DM.Web.API.Dto.Contracts;
using Conversation = DM.Web.API.Dto.Messaging.Conversation;
using Message = DM.Web.API.Dto.Messaging.Message;

namespace DM.Web.API.Services.Community
{
    /// <inheritdoc />
    public class MessagingApiService : IMessagingApiService
    {
        private readonly IConversationReadingService conversationReadingService;
        private readonly IMessageReadingService messageReadingService;
        private readonly IMapper mapper;

        /// <inheritdoc />
        public MessagingApiService(
            IConversationReadingService conversationReadingService,
            IMessageReadingService messageReadingService,
            IMapper mapper)
        {
            this.conversationReadingService = conversationReadingService;
            this.messageReadingService = messageReadingService;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ListEnvelope<Conversation>> GetConversations(PagingQuery query)
        {
            var (conversations, paging) = await conversationReadingService.Get(query);
            return new ListEnvelope<Conversation>(conversations.Select(mapper.Map<Conversation>), new Paging(paging));
        }

        /// <inheritdoc />
        public async Task<Envelope<Conversation>> GetConversation(Guid id)
        {
            var conversation = await conversationReadingService.Get(id);
            return new Envelope<Conversation>(mapper.Map<Conversation>(conversation));
        }

        /// <inheritdoc />
        public async Task<ListEnvelope<Message>> GetMessages(Guid conversationId, PagingQuery query)
        {
            var (messages, paging) = await messageReadingService.Get(conversationId, query);
            return new ListEnvelope<Message>(messages.Select(mapper.Map<Message>), new Paging(paging));
        }

        /// <inheritdoc />
        public async Task<Envelope<Message>> GetMessage(Guid messageId)
        {
            var message = await messageReadingService.Get(messageId);
            return new Envelope<Message>(mapper.Map<Message>(message));
        }
    }
}