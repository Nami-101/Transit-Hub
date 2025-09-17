using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;
using TransitHub.Models.DTOs;
using TransitHub.Repositories.Interfaces;
using TransitHub.Services.Interfaces;

namespace TransitHub.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IUnitOfWork unitOfWork, ILogger<SearchService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<TrainSearchResultDto>> SearchTrainsAsync(TrainSearchDto searchDto)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@SourceStationID", (object?)searchDto.SourceStationID ?? DBNull.Value),
                    new SqlParameter("@DestinationStationID", (object?)searchDto.DestinationStationID ?? DBNull.Value),
                    new SqlParameter("@SourceStationCode", (object?)searchDto.SourceStationCode ?? DBNull.Value),
                    new SqlParameter("@DestinationStationCode", (object?)searchDto.DestinationStationCode ?? DBNull.Value),
                    new SqlParameter("@TravelDate", searchDto.TravelDate.ToDateTime(TimeOnly.MinValue)) { SqlDbType = SqlDbType.Date },
                    new SqlParameter("@QuotaTypeID", (object?)searchDto.QuotaTypeID ?? DBNull.Value),
                    new SqlParameter("@TrainClassID", (object?)searchDto.TrainClassID ?? DBNull.Value),
                    new SqlParameter("@PassengerCount", searchDto.PassengerCount)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<TrainSearchResultDto>(
                    "sp_SearchTrains", parameters);

                _logger.LogInformation("Train search completed. Found {Count} results", result.Count());
                return result;
            }
            catch (SqlException ex) when (ex.Number >= 50010 && ex.Number <= 50015)
            {
                _logger.LogWarning("Train search validation error: {Message}", ex.Message);
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during train search");
                throw;
            }
        }

        public async Task<IEnumerable<FlightSearchResultDto>> SearchFlightsAsync(FlightSearchDto searchDto)
        {
            try
            {
                // Prepare parameters for stored procedure
                var parameters = new object[]
                {
                    new SqlParameter("@SourceAirportID", (object?)searchDto.SourceAirportID ?? DBNull.Value),
                    new SqlParameter("@DestinationAirportID", (object?)searchDto.DestinationAirportID ?? DBNull.Value),
                    new SqlParameter("@SourceAirportCode", (object?)searchDto.SourceAirportCode ?? DBNull.Value),
                    new SqlParameter("@DestinationAirportCode", (object?)searchDto.DestinationAirportCode ?? DBNull.Value),
                    new SqlParameter("@TravelDate", searchDto.TravelDate.ToDateTime(TimeOnly.MinValue)) { SqlDbType = SqlDbType.Date },
                    new SqlParameter("@FlightClassID", (object?)searchDto.FlightClassID ?? DBNull.Value),
                    new SqlParameter("@PassengerCount", searchDto.PassengerCount)
                };

                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<FlightSearchResultDto>(
                    "sp_SearchFlights", parameters);

                _logger.LogInformation("Flight search completed. Found {Count} results", result.Count());
                return result;
            }
            catch (SqlException ex) when (ex.Number >= 50010 && ex.Number <= 50015)
            {
                _logger.LogWarning("Flight search validation error: {Message}", ex.Message);
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during flight search");
                throw;
            }
        }

        public async Task<IEnumerable<StationDto>> GetAllStationsAsync()
        {
            try
            {
                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<StationDto>("sp_GetAllStations");

                _logger.LogInformation("Retrieved {Count} stations", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stations");
                throw;
            }
        }

        public async Task<IEnumerable<AirportDto>> GetAllAirportsAsync()
        {
            try
            {
                // Execute stored procedure
                var result = await _unitOfWork.ExecuteStoredProcedureAsync<AirportDto>("sp_GetAllAirports");

                _logger.LogInformation("Retrieved {Count} airports", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving airports");
                throw;
            }
        }

        public async Task<LookupDataDto> GetLookupDataAsync()
        {
            try
            {
                // This stored procedure returns multiple result sets
                // We'll need to execute it multiple times or modify the procedure to return JSON
                // For now, let's use individual calls to maintain simplicity

                var trainQuotaTypes = await _unitOfWork.ExecuteStoredProcedureAsync<TrainQuotaTypeDto>("sp_GetLookupData");
                var trainClasses = await _unitOfWork.TrainClasses.GetAllAsync();
                var flightClasses = await _unitOfWork.FlightClasses.GetAllAsync();
                var paymentModes = await _unitOfWork.PaymentModes.GetAllAsync();
                var bookingStatusTypes = await _unitOfWork.BookingStatusTypes.GetAllAsync();

                var lookupData = new LookupDataDto
                {
                    TrainQuotaTypes = trainQuotaTypes,
                    TrainClasses = trainClasses.Where(tc => tc.IsActive).Select(tc => new TrainClassDto
                    {
                        TrainClassID = tc.TrainClassID,
                        ClassName = tc.ClassName,
                        Description = tc.Description
                    }),
                    FlightClasses = flightClasses.Where(fc => fc.IsActive).Select(fc => new FlightClassDto
                    {
                        FlightClassID = fc.FlightClassID,
                        ClassName = fc.ClassName,
                        Description = fc.Description
                    }),
                    PaymentModes = paymentModes.Where(pm => pm.IsActive).Select(pm => new PaymentModeDto
                    {
                        PaymentModeID = pm.PaymentModeID,
                        ModeName = pm.ModeName,
                        Description = pm.Description
                    }),
                    BookingStatusTypes = bookingStatusTypes.Where(bst => bst.IsActive).Select(bst => new BookingStatusTypeDto
                    {
                        StatusID = bst.StatusID,
                        StatusName = bst.StatusName,
                        Description = bst.Description
                    })
                };

                _logger.LogInformation("Retrieved lookup data successfully");
                return lookupData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lookup data");
                throw;
            }
        }
    }
}