using JamWav.Application.Interfaces;
using JamWav.Domain.Entities;
using MediatR;

namespace JamWav.Application.Bands.Queries.GetAllBands;

public class GetAllBandsHandler
{
    private readonly IBandRepository _bandRepository;

    public GetAllBandsHandler(IBandRepository bandRepository)
    {
        _bandRepository = bandRepository;
    }

    public async Task<IEnumerable<Band>> Handle(GetAllBandsQuery request, CancellationToken cancellationToken)
    {
        return await _bandRepository.GetAllAsync();
    }
}