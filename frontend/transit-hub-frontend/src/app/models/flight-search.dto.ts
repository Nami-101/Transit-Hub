export interface FlightSearchDto {
  sourceAirportID?: number;
  destinationAirportID?: number;
  sourceAirportCode?: string;
  destinationAirportCode?: string;
  travelDate: string; // DateOnly as string
  flightClassID?: number;
  passengerCount: number; // 1-6, default 1
}

export interface FlightSearchResultDto {
  scheduleID: number;
  flightID: number;
  flightNumber: string;
  airline: string;
  sourceAirport: string;
  sourceAirportCode: string;
  sourceCity: string;
  destinationAirport: string;
  destinationAirportCode: string;
  destinationCity: string;
  travelDate: string;
  departureTime: string;
  arrivalTime: string;
  flightTimeMinutes: number;
  flightClass: string;
  totalSeats: number;
  availableSeats: number;
  fare: number;
  availabilityStatus: string; // Available, Limited, Sold Out
}