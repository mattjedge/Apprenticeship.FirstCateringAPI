using System;
using System.Linq;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.Core.Dtos.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FirstCateringAPI.Controllers
{
    [Route("api/[controller]")]    
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    public class MembershipCardsController : ControllerBase
    {
        private readonly IMembershipCardLogic _cardLogic;
        private readonly IConfiguration _config;

        public MembershipCardsController(IMembershipCardLogic cardLogic, IConfiguration config)
        {
            _cardLogic = cardLogic;
            _config = config;
        }

        /// <summary>
        /// Called to verify whether the card already exists in the system. Should be called before Login.
        /// Returns a MembershipCardOwnerDto which includes the cardID and employeeId (which can then be used for Login).
        /// </summary>
        /// <param name="acceptHeader"> Including the hateoas Accept header (application/vnd.catering.hateoas+json)
        /// in your request will include a links property in the response body, with the URI needed to Login the employee.</param>
        /// <param name="cardId">The guid contained on the membership card.</param>
        /// <response code="404"> Not found, no card registered with matching Card Id [perform Employees/Register]</response>
        /// <response code="200"> Ok, card found in system. Proceed to login.</response>
        [HttpGet("{cardId}/Verify")]        
        [Produces("application/json", "application/vnd.catering.hateoas+json")]
        [ProducesResponseType(404), ProducesResponseType(typeof(MembershipCardOwnerDto), 200)]
        public IActionResult VerifyCard([FromHeader(Name ="Accept")]string acceptHeader, Guid cardId)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            var acceptTypes = acceptHeader.Split(", ");

            if (!_cardLogic.MembershipCardExists(cardId))
            {
                return NotFound(new { message = "Membership card not found, please register." });
            }

            var cardAndOwner = _cardLogic.GetCardAndOwnerId(cardId);

            if (acceptTypes.Contains(hateoasHeader))
            {
                var hateoasCardAndOwner = _cardLogic.AddHateoasLinks(cardAndOwner);
                return Ok(hateoasCardAndOwner);
            }

            return Ok(cardAndOwner);
        }

        /// <summary>
        /// Called to add credit to a membership card. Ensure that card id in the URI path and the request body match.#
        /// 
        /// </summary>
        /// <param name="cardId">The Guid contained on the card.</param>
        /// <param name="acceptType">Including the hateoas accept type will include useful resource links in the response body.</param>
        /// <param name="credit">An object containing CardId (guid), Credit (the amount to add to the card), and the employees inputted PIN number.</param>
        /// <response code="200">Ok, credit added and card object returned</response>
        /// <response code="400">Bad request, path and body cardIds don't match, or request body not valid</response>
        /// <response code="404">Not found, card with matching ID not found in system.</response>
        /// <response code="401">Unauthorized, supplied PIN number does not match stored PIN.</response>
        [HttpPut("{cardId}/AddCredit", Name ="AddCredit")]
        [Produces("application/json", "application/vnd.catering.hateoas+json")]
        [ProducesResponseType(typeof(MembershipCardDto),200), ProducesResponseType(404), ProducesResponseType(400), ProducesResponseType(401)]
        public IActionResult AddCredit(Guid cardId, [FromHeader(Name ="Accept")]string acceptType, [FromBody]UpdateBalanceDto credit)
        {
            if (credit.CardId != cardId)
            {
                return BadRequest( new { message = "Path Card ID and Body Card ID don't match." });
            }

            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            var acceptTypes = acceptType.Split(", ");

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Unable to process body." });
            }
                    
            if ( !_cardLogic.MembershipCardExists(credit.CardId))
            {
                return NotFound(new { message = "Membership card not found." });
            }

            if (!_cardLogic.Authenticated(credit.PINNumber, credit.CardId))
            {
                return Unauthorized();
            }
            
            _cardLogic.AddCredit(credit);
           
            if (!_cardLogic.Save())
            {
                throw new Exception($"Adding £{credit.Credit} credit failed on save.");
            }

            var cardToReturn = _cardLogic.GetMembershipCard(credit.CardId);

            if (acceptTypes.Contains(hateoasHeader))
            {
                var hateoasMembershipCard = _cardLogic.AddHateoasLinks(cardToReturn);

                return Ok(hateoasMembershipCard);
            }
            
            return Ok(cardToReturn);
        }

        /// <summary>
        /// Used to retrieve a membership card from the database.
        /// </summary>
        /// <param name="acceptType">Including the HATEOAS accept type will return links to the card owner resource and add credit end-points.</param>
        /// <param name="cardId">The CardId guid you wish to access.</param>
        /// <response code="200">Ok, Membership Card retrieved.</response>
        /// <response code="404">Not found, no card with matching card ID found in system.</response>
        [HttpGet("{cardId}", Name = "GetMembershipCard")]
        [Produces("application/json", "application/vnd.catering.hateoas+json")]
        [ProducesResponseType(typeof(MembershipCardDto), 200), ProducesResponseType(404)]
        public IActionResult GetMembershipCard([FromHeader(Name ="Accept")] string acceptType, Guid cardId)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            var acceptTypes = acceptType.Split(", ");

            var card = _cardLogic.GetMembershipCard(cardId);

            if (card == null)
            {
                return NotFound( new { message = "Membership card not found" });
            }

            if (acceptTypes.Contains(hateoasHeader))
            {
                var hateoasCard = _cardLogic.AddHateoasLinks(card);

                return Ok(hateoasCard);
            }

            return Ok(card);
        }
    }
}