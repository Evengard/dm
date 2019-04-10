using System.Linq;
using System.Threading.Tasks;
using DM.Services.Core.Dto.Enums;
using DM.Services.Core.Parsing;
using DM.Services.DataAccess;
using DM.Services.DataAccess.SearchEngine;
using DM.Services.MessageQueuing.Dto;
using DM.Services.Search.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DM.Services.Search.Consumer.Indexing.Indexers
{
    /// <inheritdoc />
    public class CommentChangedIndexer : BaseIndexer
    {
        private readonly DmDbContext dbContext;
        private readonly IBbParserProvider bbParserProvider;
        private readonly IIndexingRepository indexingRepository;

        /// <inheritdoc />
        public CommentChangedIndexer(
            DmDbContext dbContext,
            IBbParserProvider bbParserProvider,
            IIndexingRepository indexingRepository)
        {
            this.dbContext = dbContext;
            this.bbParserProvider = bbParserProvider;
            this.indexingRepository = indexingRepository;
        }
    
        /// <inheritdoc />
        protected override EventType EventType => EventType.ChangedForumComment;

        /// <inheritdoc />
        public override async Task Index(InvokedEvent invokedEvent)
        {
            var comment = await dbContext.Comments
                .Where(c => c.CommentId == invokedEvent.EntityId)
                .Select(c => new {c.Text, c.Topic.Forum.ViewPolicy, c.Topic.ForumTopicId})
                .FirstAsync();
            await indexingRepository.Index(new SearchEntity
            {
                Id = invokedEvent.EntityId,
                ParentEntityId = comment.ForumTopicId,
                EntityType = SearchEntityType.ForumComment,
                Text = bbParserProvider.CurrentCommon.Parse(comment.Text).ToHtml(),
                AuthorizedRoles = comment.ViewPolicy.GetAuthorizedRoles()
            });
        }
    }
}