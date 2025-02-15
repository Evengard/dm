using System;
using System.Threading.Tasks;
using DM.Services.Core.Dto;
using DM.Web.API.Authentication;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Messaging;
using DM.Web.API.Services.Community;
using Microsoft.AspNetCore.Mvc;

namespace DM.Web.API.Controllers.v1.Messaging;

/// <inheritdoc />
[ApiController]
[Route("v1")]
[ApiExplorerSettings(GroupName = "Messaging")]
public class ConversationController : ControllerBase
{
    private readonly IMessagingApiService apiService;

    /// <inheritdoc />
    public ConversationController(
        IMessagingApiService apiService)
    {
        this.apiService = apiService;
    }

    /// <summary>
    /// Get list of conversations of current user
    /// </summary>
    /// <response code="200"></response>
    /// <response code="401">User must be authenticated</response>
    [HttpGet("conversations", Name = nameof(GetConversations))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(ListEnvelope<Conversation>), 200)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    public async Task<IActionResult> GetConversations([FromQuery] PagingQuery q) =>
        Ok(await apiService.GetConversations(q));

    /// <summary>
    /// Get conversation with user
    /// </summary>
    /// <response code="302"></response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="410">User not found</response>
    [HttpGet("conversations/visavi/{login}", Name = nameof(GetVisaviConversation))]
    [AuthenticationRequired]
    [ProducesResponseType(302)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetVisaviConversation(string login)
    {
        var conversation = await apiService.GetConversation(login);
        return RedirectToRoute(nameof(GetConversation), new {id = conversation.Resource.Id});
    }

    /// <summary>
    /// Get conversation of current user (by id)
    /// </summary>
    /// <response code="200"></response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="410">Dialogue not found</response>
    [HttpGet("conversations/{id}", Name = nameof(GetConversation))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(Envelope<Conversation>), 200)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetConversation(Guid id) =>
        Ok(await apiService.GetConversation(id));

    /// <summary>
    /// Mark all messages in conversation as read
    /// </summary>
    /// <response code="204"></response>
    /// <response code="410">Dialogue not found</response>
    [HttpDelete("conversations/{id}/messages/unread")]
    [AuthenticationRequired]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await apiService.MarkAsRead(id);
        return NoContent();
    }
}