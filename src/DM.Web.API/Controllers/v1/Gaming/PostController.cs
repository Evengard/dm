using System;
using System.Threading.Tasks;
using DM.Services.Core.Dto;
using DM.Web.API.Authentication;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Games;
using DM.Web.API.Services.Gaming;
using Microsoft.AspNetCore.Mvc;

namespace DM.Web.API.Controllers.v1.Gaming;

/// <inheritdoc />
[ApiController]
[Route("v1")]
[ApiExplorerSettings(GroupName = "Game")]
public class PostController : ControllerBase
{
    private readonly IPostApiService postApiService;

    /// <inheritdoc />
    public PostController(
        IPostApiService postApiService)
    {
        this.postApiService = postApiService;
    }

    /// <summary>
    /// Get list of posts in room
    /// </summary>
    /// <param name="id"></param>
    /// <param name="q"></param>
    /// <response code="200"></response>
    /// <response code="410">Room not found</response>
    [HttpGet("rooms/{id}/posts", Name = nameof(GetPosts))]
    [ProducesResponseType(typeof(ListEnvelope<Post>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetPosts(Guid id, [FromQuery] PagingQuery q) =>
        Ok(await postApiService.Get(id, q));

    /// <summary>
    /// Create new post in room
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <response code="201"></response>
    /// <response code="400">Some of post parameters were invalid</response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not allowed to create post in this room</response>
    /// <response code="410">Room not found</response>
    [HttpPost("rooms/{id}/posts", Name = nameof(PostPost))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(Envelope<Post>), 201)]
    [ProducesResponseType(typeof(BadRequestError), 400)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 403)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> PostPost(Guid id, [FromBody] Post post)
    {
        var result = await postApiService.Create(id, post);
        return CreatedAtRoute(nameof(GetPost),
            new {id = result.Resource.Id}, result);
    }

    /// <summary>
    /// Get post
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200"></response>
    /// <response code="410">Post not found</response>
    [HttpGet("posts/{id}", Name = nameof(GetPost))]
    [ProducesResponseType(typeof(Envelope<Post>), 200)]
    public async Task<IActionResult> GetPost(Guid id) => Ok(await postApiService.Get(id));

    /// <summary>
    /// Update post
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <response code="200"></response>
    /// <response code="400">Some of post changed properties were invalid or passed id was not recognized</response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not authorized to change some properties of this post</response>
    /// <response code="410">Post not found</response>
    [HttpPatch("posts/{id}", Name = nameof(PutPost))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(Envelope<Post>), 200)]
    [ProducesResponseType(typeof(BadRequestError), 400)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 403)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> PutPost(Guid id, [FromBody] Post post) =>
        Ok(await postApiService.Update(id, post));

    /// <summary>
    /// Delete post
    /// </summary>
    /// <param name="id"></param>
    /// <response code="204"></response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not allowed to remove the post</response>
    /// <response code="410">Post not found</response>
    [HttpDelete("posts/{id}", Name = nameof(DeletePost))]
    [AuthenticationRequired]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 403)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        await postApiService.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Get list of reviews of post
    /// </summary>
    /// <response code="200"></response>
    /// <response code="410">Post not found</response>
    [HttpGet("posts/{id}/reviews", Name = nameof(GetPostReviews))]
    [ProducesResponseType(typeof(ListEnvelope<Vote>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    // TODO: Get list of reviews of post
    public Task<IActionResult> GetPostReviews(Guid id) => throw new NotImplementedException();

    /// <summary>
    /// Create new review of post
    /// </summary>
    /// <response code="201"></response>
    /// <response code="400">Some of vote parameters were invalid</response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not allowed to review for the post</response>
    /// <response code="409">User already reviewed for this post</response>
    /// <response code="410">Post not found</response>
    [HttpPost("posts/{id}/reviews", Name = nameof(PostReviews))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(Envelope<Vote>), 201)]
    [ProducesResponseType(typeof(BadRequestError), 400)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 403)]
    [ProducesResponseType(typeof(GeneralError), 409)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    // TODO: Create new review of post
    public Task<IActionResult> PostReviews(Guid id, [FromBody] Vote vote) => throw new NotImplementedException();

    /// <summary>
    /// Get post review
    /// </summary>
    /// <response code="200"></response>
    /// <response code="410">Review not found</response>
    [HttpGet("postreview/{id}", Name = nameof(GetPostReview))]
    [ProducesResponseType(typeof(Envelope<Vote>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    // TODO: Get post review
    public Task<IActionResult> GetPostReview(Guid id) => throw new NotImplementedException();

    /// <summary>
    /// Update post review
    /// </summary>
    /// <response code="200"></response>
    /// <response code="410">Review not found</response>
    [HttpPatch("postreview/{id}", Name = nameof(PatchPostReview))]
    [ProducesResponseType(typeof(Envelope<Vote>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    // TODO: Update post review
    public Task<IActionResult> PatchPostReview(Guid id) => throw new NotImplementedException();

    /// <summary>
    /// Delete post review
    /// </summary>
    /// <response code="200"></response>
    /// <response code="410">Review not found</response>
    [HttpDelete("postreview/{id}", Name = nameof(DeletePostReview))]
    [ProducesResponseType(typeof(Envelope<Vote>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    // TODO: Delete post review
    public Task<IActionResult> DeletePostReview(Guid id) => throw new NotImplementedException();
}