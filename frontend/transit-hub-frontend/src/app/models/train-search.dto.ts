export interface TrainSearchDto {
  sourceStationID?: number;
  destinationStationID?: number;
  sourceStationCode?: string;
  destinationStationCode?: string;
  travelDate: string; // DateOnly as string
  quotaTypeID?: number;
  trainClassID?: number;
  passengerCount: number; // 1-6, default 1
}

export interface TrainSearchResultDto {
  scheduleID: number;
  trainID: number;
  trainName: string;
  trainNumber: string;
  sourceStation: string;
  sourceStationCode: string;
  destinationStation: string;
  destinationStationCode: string;
  travelDate: string;
  departureTime: string;
  arrivalTime: string;
  journeyTimeMinutes: number;
  quotaName: string;
  trainClass: string;
  totalSeats: number;
  availableSeats: number;
  fare: number;
  availabilityStatus: string; // Available, Limited, Waitlist
  availableOrWaitlistPosition: number;
}