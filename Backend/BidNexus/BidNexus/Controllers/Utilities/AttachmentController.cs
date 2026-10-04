using API.Abstraction.Utilities;
using Core.Models.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers.Utilities
{
    [ApiController]
    [Route("api/attachments")]
    public class AttachmentController(IAttachmentService attachmentService) : ControllerBase
    {
        private readonly IAttachmentService _attachmentService = attachmentService;

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AttachmentDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AttachmentDataModel>> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _attachmentService.UploadAsync(file, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var file = await _attachmentService.GetFileAsync(id, cancellationToken);
            if (file == null)
            {
                return NotFound(new { message = $"Attachment with ID {id} was not found." });
            }

            Response.Headers.CacheControl = "public,max-age=86400";
            return File(file.Value.Stream, file.Value.ContentType, file.Value.FileName);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _attachmentService.DeleteAttachmentAsync(id, cancellationToken);
            if (!success)
            {
                return NotFound(new { message = $"Attachment with ID {id} was not found." });
            }

            return NoContent();
        }
    }
}
