using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using DM.Services.Authentication.Implementation.UserIdentity;
using DM.Services.Common.Authorization;
using DM.Services.Common.BusinessProcesses.Likes;
using DM.Services.Core.Dto;
using DM.Services.Core.Dto.Enums;
using DM.Services.Core.Exceptions;
using DM.Services.DataAccess.BusinessObjects.Common;
using DM.Services.Forum.Authorization;
using DM.Services.Forum.BusinessProcesses.Commentaries.Reading;
using DM.Services.Forum.BusinessProcesses.Likes;
using DM.Services.Forum.BusinessProcesses.Topics.Reading;
using DM.Services.Forum.Dto.Output;
using DM.Services.Forum.Tests.Dsl;
using DM.Services.MessageQueuing.GeneralBus;
using DM.Tests.Core;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using Xunit;
using Comment = DM.Services.Common.Dto.Comment;

namespace DM.Services.Forum.Tests.BusinessProcesses.Likes;

public class LikeServiceForCommentsShould : UnitTestBase
{
    private readonly LikeService service;
    private readonly ISetup<ICommentaryReadingService, Task<Comment>> commentReading;
    private readonly ISetup<IIdentity, AuthenticatedUser> currentUser;
    private readonly Mock<ILikeFactory> factory;
    private readonly Mock<ILikeRepository> likeRepository;
    private readonly Mock<IInvokedEventProducer> publisher;

    public LikeServiceForCommentsShould()
    {
        var commentReadingService = Mock<ICommentaryReadingService>();
        commentReading = commentReadingService.Setup(s => s.Get(It.IsAny<Guid>()));

        var intentionManager = Mock<IIntentionManager>();
        intentionManager
            .Setup(m => m.ThrowIfForbidden(TopicIntention.Like, It.IsAny<Topic>()));
        var identityProvider = Mock<IIdentityProvider>();
        var identity = Mock<IIdentity>();
        currentUser = identity.Setup(i => i.User);
        identityProvider.Setup(p => p.Current).Returns(identity.Object);

        factory = Mock<ILikeFactory>();
        likeRepository = Mock<ILikeRepository>();
        publisher = Mock<IInvokedEventProducer>();
        service = new LikeService(Mock<ITopicReadingService>().Object, commentReadingService.Object,
            intentionManager.Object, identityProvider.Object, factory.Object,
            likeRepository.Object, publisher.Object);
    }

    [Fact]
    public async Task ThrowConflictExceptionWhenUserTriesToLikeTopicTwice()
    {
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        commentReading.ReturnsAsync(new Comment
        {
            Likes = new List<GeneralUser>
            {
                new() {UserId = userId},
                new() {UserId = Guid.NewGuid()}
            }
        });
        currentUser.Returns(Create.User(userId).Please);

        var err = await service.Awaiting(s => s.LikeComment(commentId))
            .Should().ThrowAsync<HttpException>();
        err.And.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task SaveInRepositoryAndPublishMessageAndReturnUserWhenLikes()
    {
        var commentId = Guid.NewGuid();
        commentReading.ReturnsAsync(new Comment
        {
            Id = commentId,
            Likes = new List<GeneralUser>
            {
                new() {UserId = Guid.NewGuid()},
                new() {UserId = Guid.NewGuid()}
            }
        });
        var user = Create.User().Please();
        currentUser.Returns(user);

        var likeId = Guid.NewGuid();
        var like = new Like {LikeId = likeId};
        factory
            .Setup(f => f.Create(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(like);
        likeRepository
            .Setup(r => r.Add(It.IsAny<Like>()))
            .Returns(Task.CompletedTask);
        publisher
            .Setup(p => p.Send(It.IsAny<EventType>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        var actual = await service.LikeComment(commentId);
        actual.Should().Be(user);

        likeRepository.Verify(r => r.Add(like), Times.Once);
        likeRepository.VerifyNoOtherCalls();

        publisher.Verify(p => p.Send(EventType.LikedForumComment, likeId), Times.Once);
        publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ThrowConflictExceptionWhenUserTriesToDislikeHeNeverLiked()
    {
        var commentId = Guid.NewGuid();
        commentReading.ReturnsAsync(new Comment
        {
            Likes = new List<GeneralUser>
            {
                new() {UserId = Guid.NewGuid()}
            }
        });
        currentUser.Returns(Create.User().Please);

        var err = await service.Awaiting(s => s.DislikeComment(commentId))
            .Should().ThrowAsync<HttpException>();
        err.And.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task SaveInRepositoryWhenDislikes()
    {
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        commentReading.ReturnsAsync(new Comment
        {
            Id = commentId,
            Likes = new List<GeneralUser>
            {
                new() {UserId = userId},
                new() {UserId = Guid.NewGuid()}
            }
        });
        var user = Create.User(userId).Please();
        currentUser.Returns(user);

        likeRepository
            .Setup(r => r.Delete(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        await service.DislikeComment(commentId);

        likeRepository.Verify(r => r.Delete(commentId, userId), Times.Once);
        likeRepository.VerifyNoOtherCalls();
    }
}