using System.Threading.Tasks;
using DM.Web.API.Authentication;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Fora;
using DM.Web.API.Dto.Users;
using DM.Web.API.Services.Fora;
using Microsoft.AspNetCore.Mvc;

namespace DM.Web.API.Controllers.v1.Forums;

/// <inheritdoc />
[ApiController]
[Route("v1/boards")]
[ApiExplorerSettings(GroupName = "Forum")]
public class BoardController : ControllerBase
{
    private readonly IForumApiService forumApiService;
    private readonly ITopicApiService topicApiService;
    private readonly ICommentApiService commentApiService;
    private readonly IModeratorsApiService moderatorsApiService;

    /// <inheritdoc />
    public BoardController(
        IForumApiService forumApiService,
        ITopicApiService topicApiService,
        ICommentApiService commentApiService,
        IModeratorsApiService moderatorsApiService)
    {
        this.forumApiService = forumApiService;
        this.topicApiService = topicApiService;
        this.commentApiService = commentApiService;
        this.moderatorsApiService = moderatorsApiService;
    }

    /// <summary>
    /// Get list of all boards
    /// </summary>
    /// <response code="200"></response>
    [HttpGet(Name = nameof(GetBoards))]
    [ProducesResponseType(typeof(ListEnvelope<Forum>), 200)]
    public async Task<IActionResult> GetBoards() => Ok(await forumApiService.Get());

    /// <summary>
    /// Get board
    /// </summary>
    /// <param name="id">Forum id</param>
    /// <response code="200"></response>
    /// <response code="410">Forum not found</response>
    [HttpGet("{id}", Name = nameof(GetBoard))]
    [ProducesResponseType(typeof(Envelope<Forum>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetBoard(string id) => Ok(await forumApiService.Get(id));

    /// <summary>
    /// Mark all comments on board as read
    /// </summary>
    /// <param name="id">Forum id</param>
    /// <response code="204"></response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="410">Forum not found</response>
    [HttpDelete("{id}/comments/unread", Name = nameof(ReadBoardComments))]
    [AuthenticationRequired]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> ReadBoardComments(string id)
    {
        await commentApiService.MarkAsRead(id);
        return NoContent();
    }

    /// <summary>
    /// Get board moderators
    /// </summary>
    /// <param name="id">Forum id</param>
    /// <response code="200"></response>
    /// <response code="410">Forum not found</response>
    [HttpGet("{id}/moderators", Name = nameof(GetBoardModerators))]
    [ProducesResponseType(typeof(ListEnvelope<User>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetBoardModerators(string id) => Ok(await moderatorsApiService.GetModerators(id));
}