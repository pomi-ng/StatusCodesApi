using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StatusCodesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedirectTestController : ControllerBase
    {
        /// <summary>
        /// Endpoint target che accetta solo POST.
        /// Restituisce un messaggio di conferma.
        /// </summary>
        [HttpPost("target")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public IActionResult TargetEndpoint()
        {
            return Ok(new { message = "POST processed correctly at target endpoint." });
        }

        /// <summary>
        /// Endpoint che reindirizza con un redirect 301 (Moved Permanently).
        /// Molti client convertono il metodo POST in GET, causando un 405 sul target.
        /// </summary>
        [HttpPost("redirect301")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
        public IActionResult Redirect301()
        {
            // RedirectPermanent restituisce un 301 Moved Permanently.
            // L'URL di destinazione è relativo; il framework lo trasforma in URL assoluto.
            return RedirectPermanent(Url.Action(nameof(TargetEndpoint), "RedirectTest"));
        }

        /// <summary>
        /// Endpoint che reindirizza con un redirect 308 (Permanent Redirect),
        /// preservando il metodo POST.
        /// </summary>
        [HttpPost("redirect308")]
        [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
        public IActionResult Redirect308()
        {
            // RedirectPermanentPreserveMethod restituisce un 308,
            // mantenendo il metodo POST.
            return RedirectPermanentPreserveMethod(Url.Action(nameof(TargetEndpoint), "RedirectTest"));
        }
    }
}
