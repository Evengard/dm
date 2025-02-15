using System;
using System.Threading.Tasks;
using DM.Web.API.Authentication;
using DM.Web.API.Dto.Community;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Services.Community;
using Microsoft.AspNetCore.Mvc;

namespace DM.Web.API.Controllers.v1.Community;

/// <inheritdoc />
[ApiController]
[Route("v1/websitereviews")]
[ApiExplorerSettings(GroupName = "Community")]
public class WebsiteReviewController : ControllerBase
{
    private readonly IReviewApiService reviewApiService;

    /// <inheritdoc />
    public WebsiteReviewController(
        IReviewApiService reviewApiService)
    {
        this.reviewApiService = reviewApiService;
    }

    /// <summary>
    /// Get list of website reviews
    /// </summary>
    /// <param name="q"></param>
    /// <response code="200"></response>
    [HttpGet(Name = nameof(GetWebsiteReviews))]
    [ProducesResponseType(typeof(ListEnvelope<Review>), 200)]
    public async Task<IActionResult> GetWebsiteReviews([FromQuery] ReviewsQuery q) => Ok(await reviewApiService.Get(q));

    /// <summary>
    /// Create new website review
    /// </summary>
    /// <response code="201"></response>
    /// <response code="400">Some review parameters were invalid</response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not authorized to create reviews</response>
    [HttpPost(Name = nameof(CreateWebsiteReview))]
    [AuthenticationRequired]
    public async Task<IActionResult> CreateWebsiteReview([FromBody] Review review)
    {
        var result = await reviewApiService.Create(review);
        return CreatedAtRoute(nameof(GetWebsiteReview), new {id = result.Resource.Id}, result);
    }

    /// <summary>
    /// Get website review
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200"></response>
    /// <response code="410">Review not found</response>
    [HttpGet("{id}", Name = nameof(GetWebsiteReview))]
    [ProducesResponseType(typeof(Envelope<Review>), 200)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> GetWebsiteReview(Guid id) => Ok(await reviewApiService.Get(id));

    /// <summary>
    /// Update website review
    /// </summary>
    /// <response code="200"></response>
    /// <response code="400">Some review parameters were invalid</response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="403">User is not authorized to update this review</response>
    /// <response code="410">Review not found</response>
    [HttpPatch("{id}", Name = nameof(PutWebsiteReview))]
    [AuthenticationRequired]
    [ProducesResponseType(typeof(Envelope<Review>), 200)]
    [ProducesResponseType(typeof(BadRequestError), 400)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 403)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> PutWebsiteReview(Guid id, [FromBody] Review review) =>
        Ok(await reviewApiService.Update(id, review));

    /// <summary>
    /// Delete website review
    /// </summary>
    /// <response code="204"></response>
    /// <response code="401">User must be authenticated</response>
    /// <response code="410">Review not found</response>
    [HttpDelete("{id}", Name = nameof(DeleteWebsiteReview))]
    [AuthenticationRequired]
    [ProducesResponseType(240)]
    [ProducesResponseType(typeof(GeneralError), 401)]
    [ProducesResponseType(typeof(GeneralError), 410)]
    public async Task<IActionResult> DeleteWebsiteReview(Guid id)
    {
        await reviewApiService.Delete(id);
        return NoContent();
    }
}