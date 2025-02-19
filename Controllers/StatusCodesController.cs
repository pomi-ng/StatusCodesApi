using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StatusCodesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusCodesController : ControllerBase
    {
        /// <summary>
        /// Restituisce 200 OK con un messaggio di successo.
        /// </summary>
        [HttpGet("ok")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetOk() =>
            Ok(new { message = "Everything is OK!" });

        /// <summary>
        /// Crea una nuova risorsa e restituisce 201 Created.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public IActionResult CreateResource([FromForm] ResourceRequest request,[FromQuery] int id)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { message = "Bad Request: 'Name' is required." });
            }
            // Simula la creazione della risorsa con id 1.
            return CreatedAtAction(nameof(GetResource), new { id = id }, new { message = "Resource created"+id.ToString(), resource = request });
        }

        /// <summary>
        /// Elimina una risorsa e restituisce 204 No Content.
        /// </summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteResource(int id)
        {
            // Simula l'eliminazione della risorsa.
            return NoContent();
        }

        /// <summary>
        /// Verifica il parametro query 'value' e restituisce 400 Bad Request se mancante o non numerico.
        /// </summary>
        [HttpGet("badrequest")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public IActionResult GetBadRequest([FromQuery] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest(new { message = "Bad Request: 'value' query parameter is required." });
            }
            if (!int.TryParse(value, out _))
            {
                return BadRequest(new { message = "Bad Request: 'value' must be an integer." });
            }
            return Ok(new { message = "Valid query parameter received." });
        }

        /// <summary>
        /// Controlla la presenza dell'header Authorization e restituisce 401 se mancante.
        /// </summary>
        [HttpGet("unauthorized")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public IActionResult GetUnauthorized()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized(new { message = "Unauthorized: Missing Authorization header." });
            }
            return Ok(new { message = "Authorization header present." });
        }

        /// <summary>
        /// Valida l'header Authorization e restituisce 403 se il token è errato.
        /// </summary>
        [HttpGet("forbidden")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetForbidden()
        {
            if (Request.Headers.TryGetValue("Authorization", out var token))
            {
                if (!token.ToString().Equals("Bearer VALID_TOKEN", StringComparison.Ordinal))
                {
                    return Forbid();
                }
                return Ok(new { message = "Valid token provided." });
            }
            return Unauthorized(new { message = "Unauthorized: Missing Authorization header." });
        }

        /// <summary>
        /// Restituisce 404 Not Found se la risorsa non esiste.
        /// </summary>
        [HttpGet("notfound/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public IActionResult GetResource(int id)
        {
            if (id != 1) // Simuliamo che esista solo la risorsa con id 1.
            {
                return NotFound(new { message = $"Resource with id {id} not found." });
            }
            return Ok(new { id, message = "Resource found." });
        }

        /// <summary>
        /// Simula un conflitto nella creazione della risorsa.
        /// </summary>
        [HttpPost("conflict")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public IActionResult CreateDuplicateResource([FromBody] ResourceRequest request)
        {
            if (request.Name.Equals("Duplicate", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = "Conflict: Resource already exists." });
            }
            return CreatedAtAction(nameof(GetResource), new { id = 2 }, new { message = "Resource created", resource = request });
        }

        /// <summary>
        /// Restituisce 422 Unprocessable Entity se i dati sono semanticamente errati.
        /// </summary>
        [HttpPost("unprocessable")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult CreateResourceUnprocessable([FromBody] ResourceRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name) || request.Name.Length < 3)
            {
                return UnprocessableEntity(new { message = "Unprocessable Entity: 'Name' must be at least 3 characters long." });
            }
            return CreatedAtAction(nameof(GetResource), new { id = 3 }, new { message = "Resource created", resource = request });
        }

        /// <summary>
        /// Simula il superamento del rate limit, restituendo 429 Too Many Requests.
        /// </summary>
        [HttpGet("toomany")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status429TooManyRequests)]
        public IActionResult GetTooManyRequests([FromQuery] int requests)
        {
            if (requests > 5)
            {
                return StatusCode(429, new { message = "Too Many Requests: Rate limit exceeded." });
            }
            return Ok(new { message = "Request accepted.", requests });
        }

        /// <summary>
        /// Simula un errore interno del server, restituendo 500 Internal Server Error.
        /// </summary>
        [HttpGet("internalerror")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetInternalError([FromQuery] bool trigger)
        {
            if (trigger )
            {
                throw new Exception("Simulated internal server error.");
            }
            return Ok(new { message = "No error triggered." });
        }

        /// <summary>
        /// Simula un errore di Bad Gateway (502).
        /// </summary>
        [HttpGet("badgateway")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status502BadGateway)]
        public IActionResult GetBadGateway([FromQuery] bool simulate)
        {
            if (simulate)
            {
                return StatusCode(502, new { message = "Bad Gateway: Failed to retrieve data from external service." });
            }
            return Ok(new { message = "External service call successful." });
        }

        /// <summary>
        /// Simula un errore Service Unavailable (503).
        /// </summary>
        [HttpGet("serviceunavailable")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult GetServiceUnavailable([FromQuery] bool maintenance)
        {
            if (maintenance)
            {
                return StatusCode(503, new { message = "Service Unavailable: System is under maintenance." });
            }
            return Ok(new { message = "Service is available." });
        }

        /// <summary>
        /// Simula un errore Gateway Timeout (504).
        /// </summary>
        [HttpGet("gatewaytimeout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status504GatewayTimeout)]
        public IActionResult GetGatewayTimeout([FromQuery] bool timeout)
        {
            if (timeout)
            {
                return StatusCode(504, new { message = "Gateway Timeout: External service did not respond in time." });
            }
            return Ok(new { message = "External service responded in time." });
        }

        [HttpGet("negotiate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status406NotAcceptable)]
        public IActionResult ContentNegotiation()
        {
            // Controlla se l'header Accept contiene "application/json"
            if (!Request.Headers.TryGetValue("Accept", out var acceptHeader) ||
                !acceptHeader.ToString().Contains("application/json"))
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, 
                    new { message = "Not Acceptable: Only application/json responses are supported." });
            }

            return Ok(new { message = "Content negotiation successful. You accept JSON." });
        }
        [HttpPost("validate-content")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status415UnsupportedMediaType)]
        public IActionResult ValidateContentType([FromBody] ResourceRequest request)
        {
            // Controlla se il Content-Type è "application/json"
            if (!Request.Headers.TryGetValue("Content-Type", out var contentType) ||
                !contentType.ToString().Equals("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType, 
                    new { message = "Unsupported Media Type: Only application/json is accepted." });
            }

            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { message = "Bad Request: 'Name' is required." });
            }

            // Simulazione creazione risorsa
            return CreatedAtAction(nameof(GetResource), new { id = 1 }, new { message = "Resource created", resource = request });
        }
        /// <summary>
        /// Esegue un redirect permanente (301) verso un'altra URL.
        /// </summary>
        /// <returns>301 Moved Permanently</returns>
        [HttpGet("notHereAnymore")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
        public IActionResult GetRedirect301()
        {
            // RedirectPermanent restituisce un 301.
            return RedirectPermanent("https://example.com/new-location");
        }

        /// <summary>
        /// Esegue un redirect permanente (308) verso un'altra URL, preservando il metodo HTTP.
        /// </summary>
        /// <returns>308 Permanent Redirect</returns>
        [HttpGet("willRedirectToTarget")]
        [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
        public IActionResult GetRedirect308()
        {
            // RedirectPermanentPreserveMethod restituisce un 308.
            return RedirectPermanentPreserveMethod("https://example.com/new-location");
        }

        
    }

    public class ResourceRequest
    {
        public string Name { get; set; }
    }
}
