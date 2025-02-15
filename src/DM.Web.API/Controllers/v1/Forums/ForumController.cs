using System;
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
[Route("v1/forum")]
[ApiExplorerSettings(GroupName = "Forum")]
public class ForumController : ControllerBase
{
	private readonly IForumApiService forumApiService;

	/// <inheritdoc />
	public ForumController(
		IForumApiService forumApiService)
	{
		this.forumApiService = forumApiService;
	}

	/// <summary>
	/// Get list of all boards
	/// </summary>
	/// <response code="200"></response>
	[HttpGet(Name = nameof(GetForum))]
	[ProducesResponseType(typeof(ListEnvelope<Forum>), 200)]
	// TODO: Get list of all boards
	public Task<IActionResult> GetForum() => throw new NotImplementedException();

	/// <summary>
	/// Mark all comments on forum as read
	/// </summary>
	/// <param name="id">Forum id</param>
	/// <response code="204"></response>
	/// <response code="401">User must be authenticated</response>
	/// <response code="410">Forum not found</response>
	[HttpDelete("comments/unread", Name = nameof(ReadForumComments))]
	[AuthenticationRequired]
	[ProducesResponseType(204)]
	[ProducesResponseType(typeof(GeneralError), 401)]
	[ProducesResponseType(typeof(GeneralError), 410)]
	// TODO: Mark all comments on forum as read
	public Task<IActionResult> ReadForumComments(string id) => throw new NotImplementedException();
}