using Microsoft.AspNetCore.Mvc;
using WebApp.Model.Images;
using WebApp.Model;
using WebApp.Model.Receipts;
using WebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    /// <summary>
    /// REST API for OCR and contextualize receipts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        /// <summary>
        /// Validate your API Key
        /// </summary>
        /// <param name="apikey">Rez API Key</param>
        /// <returns></returns>
        [HttpGet("Check")]
        public IActionResult Test([FromHeader] string apikey)
        {
            var user = GetApiUser(apikey);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok();
        }

        /// <summary>
        /// Get the status of a report
        /// </summary>
        /// <param name="reportId">ID of report</param>
        /// <param name="apikey">Rez API Key</param>
        /// <returns></returns>
        [HttpGet("Report/{reportId}")]
        public ActionResult<ScanTask> Get(string reportId, [FromHeader] string apikey)
        {
            var user = GetApiUser(apikey);
            if (user == null)
            {
                return Unauthorized();
            }

            var job = RezApi.Jobs.OcrJobs.FirstOrDefault(c => c.Id == reportId);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        /// <summary>
        /// Ocr and contextualize receipts
        /// </summary>
        /// <param name="file">Image of your receipt</param>
        /// <param name="apikey">Rez API Key</param>
        /// <param name="mockMode">Return dummy data for development purpose</param>
        /// <returns></returns>
        [HttpPost("Scan")]
        public async Task<ActionResult<ScanTask>> PostCmFiles(IFormFile file, [FromHeader] string apikey, [FromQuery] bool mockMode = true)
        {
            try
            {
                var user = GetApiUser(apikey);
                if (user == null)
                {
                    return Unauthorized();
                }

                var imageFile = file;
                var imageInfo = new ImageInfo(imageFile)
                {
                    UserId = user?.Id ?? "",
                };
                await imageInfo.CopyToLocalPath(imageFile);
                imageInfo.ProcessImage();

                ScanTask scanTask;
                if(mockMode)
                {
                    scanTask = await RezApi.Jobs.GetTestClient();
                }
                else
                {
                    scanTask = await RezApi.Jobs.GetGptClient();
                }
                await scanTask.DoJob(imageInfo);
                return Ok(scanTask);
            }
            catch (Exception e)
            {
                return Problem(e.Message, "", 400);
            }
        }

        private User? GetApiUser(string key)
        {
            return RezApi.Users.AllUsers.FirstOrDefault(c => c.ApiKeys.Any(c=>c.ApiKey == key && c.IsActive) );
        }
    }
}
