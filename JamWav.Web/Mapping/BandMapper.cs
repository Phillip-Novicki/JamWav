using JamWav.Domain.Entities;
using JamWav.Web.Models;

namespace JamWav.Web.Mapping;

public static class BandMapper
{
    public static BandResponse ToResponse(this Band band) => new BandResponse
    {
        Id = band.Id,
        Name = band.Name,
        Genre = band.Genre,
        Origin = band.Origin,
        CreatedAt = band.CreatedAt
    };

    public static Band ToEntity(this CreateBandRequest request) => new Band(
        request.Name,
        request.Genre,
        request.Origin
    );

    public static void UpdateFrom(this Band entity, UpdateBandRequest request)
    {
        entity.Name = request.Name;
        entity.Genre = request.Genre;
        entity.Origin = request.Origin;
    }
}