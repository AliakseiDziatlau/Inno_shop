using AutoMapper;
using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Core.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(request.Page, request.PageSize);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}