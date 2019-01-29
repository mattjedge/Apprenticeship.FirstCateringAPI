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


        [HttpGet("{cardId}/Verify")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(MembershipCardOwnerDto),200)]
        [Produces("application/json", "application/vnd.catering.hateoas+json")]
        public IActionResult VerifyCard([FromHeader(Name ="Accept")]string acceptHeader, Guid cardId)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            var acceptTypes = acceptHeader.Split(", ");

            if (!_cardLogic.MembershipCardExists(cardId))
            {
                return NotFound("Membership card not found.");
            }

            var cardAndOwner = _cardLogic.GetCardAndOwnerId(cardId);

            if (acceptTypes.Contains(hateoasHeader))
            {
                var hateoasCardAndOwner = _cardLogic.AddHateoasLinks(cardAndOwner);
                return Ok(hateoasCardAndOwner);
            }

            return Ok(cardAndOwner);
        }


        [HttpPut("{cardId}/TopUp")]
        public IActionResult TopUp()
        {
            return Ok();
        }

        
        [HttpGet("{cardId}", Name = "GetMembershipCard")]
        public IActionResult GetMembershipCard([FromHeader(Name ="Accept")] string acceptType, Guid cardId)
        {
            var hateoasHeader = _config.GetValue<string>("AppSettings:HateoasAcceptType");
            var acceptTypes = acceptType.Split(", ");

            var card = _cardLogic.GetMembershipCard(cardId);

            if (card == null)
            {
                return BadRequest();
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