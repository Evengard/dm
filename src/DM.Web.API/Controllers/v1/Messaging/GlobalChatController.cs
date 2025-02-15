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
[Route("v1/globalchat")]
[ApiExplorerSettings(GroupName = "Messaging")]
public class GlobalChatController : ControllerBase
{
    private readonly IMessagingApiService apiService;

    /// <inheritdoc />
    public GlobalChatController(
        IMessagingApiService apiService)
    {
        this.apiService = apiService;
    }

    /// <summary>
    /// Get global chat
    /// </summary>
    /// <response code="200"></response>
    [HttpGet(Name = nameof(GetChat))]
    [ProducesResponseType(204)]
    // TODO: Get global chat
    public Task<IActionResult> GetChat() => throw new NotImplementedException();
}