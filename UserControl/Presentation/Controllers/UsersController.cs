using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;
namespace UserControl.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase 
{ 
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
                _mediator = mediator;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
                var query = new GetUserByIdCommand(id);
                var userDto = await _mediator.Send(query);
                return Ok(userDto);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
                var command = new DeleteUserCommand(id);
                await _mediator.Send(command);
                return NoContent();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto requestDto)
        {
                var command = new UpdateUserCommand(id, requestDto.Name, requestDto.Email, requestDto.Role);
                await _mediator.Send(command);
                return NoContent();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
                var query = new GetAllUsersCommand(page, pageSize);
                var users = await _mediator.Send(query);
                return Ok(users); 
        }
        
        /*
         Only admin has a permission to activate user. If user was activated, all his products,
         that had IsDeleted field in DB with value true, becomes marked as IsDeleted = false 
         */
        [Authorize(Roles = "Admin")]
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateUser(int id)
        {
                var command = new ActivateUserCommand(id);
                await _mediator.Send(command);
                return NoContent(); 
        }
        
        /*
         Only admin has a permission to deactivate user. If user was deactivated, all his products,
         in DB would be marked as IsDeleted = true 
         */
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
                var command = new DeactivateUserCommand(id);
                await _mediator.Send(command);
                return NoContent();
        }
}