using TrainBooking.Models.DTOs;

namespace TrainBooking.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<TrainSearchResultDto>> SearchTrainsAsync(TrainSearchDto searchDto);
        Task<IEnumerable<FlightSearchResultDto>> SearchFlightsAsync(FlightSearchDto searchDto);
        Task<IEnumerable<StationDto>> GetAllStationsAsync();
        Task<IEnumerable<AirportDto>> GetAllAirportsAsync();
        Task<LookupDataDto> GetLookupDataAsync();
    }
}