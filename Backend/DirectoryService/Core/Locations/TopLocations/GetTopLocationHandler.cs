using Contracts.DTOs;
using Contracts.Errors;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;

namespace Core.Locations.TopLocations
{
    internal class GetTopLocationHandler : ICommandHandler<IEnumerable<TopLocationDto>, GetTopLocationCommand>
    {
        private readonly ILocationReadRepository _locationReadRepository;

        public GetTopLocationHandler(ILocationReadRepository locationReadRepository)
        {
            _locationReadRepository = locationReadRepository;
        }

        public async Task<Result<IEnumerable<TopLocationDto>, AppError>> HandlerAsync(GetTopLocationCommand command, CancellationToken cancellationToken)
        {
            var topLocations = await _locationReadRepository.GetTopLocationsAsync(command.Limit, cancellationToken);

            return Result.Success<IEnumerable<TopLocationDto>, AppError>(topLocations);
        }
    }
}
