export interface TrainQuotaTypeDto {
  quotaTypeID: number;
  quotaName: string;
  description?: string;
}

export interface TrainClassDto {
  trainClassID: number;
  className: string;
  description?: string;
}

export interface FlightClassDto {
  flightClassID: number;
  className: string;
  description?: string;
}

export interface PaymentModeDto {
  paymentModeID: number;
  modeName: string;
  description?: string;
}

export interface BookingStatusTypeDto {
  statusID: number;
  statusName: string;
  description?: string;
}

export interface LookupDataDto {
  trainQuotaTypes: TrainQuotaTypeDto[];
  trainClasses: TrainClassDto[];
  flightClasses: FlightClassDto[];
  paymentModes: PaymentModeDto[];
  bookingStatusTypes: BookingStatusTypeDto[];
}