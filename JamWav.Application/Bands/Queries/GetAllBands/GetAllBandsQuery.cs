using JamWav.Domain.Entities;
using MediatR;

namespace JamWav.Application.Bands.Queries.GetAllBands;
public record GetAllBandsQuery() : IRequest<IEnumerable<Band>>;
