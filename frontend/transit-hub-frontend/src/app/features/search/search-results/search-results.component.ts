import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { TrainSearchResultDto } from '../../../models/train-search.dto';
import { FlightSearchResultDto } from '../../../models/flight-search.dto';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.css']
})
export class SearchResultsComponent {
  @Input() results: (TrainSearchResultDto | FlightSearchResultDto)[] = [];
  @Input() searchMode: 'train' | 'flight' = 'train';

  // Define column definitions for train results
  trainDisplayedColumns: string[] = [
    'trainNumber', 'trainName', 'route', 'timing', 'class', 'quota', 
    'availability', 'fare', 'actions'
  ];

  // Define column definitions for flight results  
  flightDisplayedColumns: string[] = [
    'flightNumber', 'airline', 'route', 'timing', 'class', 
    'availability', 'fare', 'actions'
  ];

  get displayedColumns(): string[] {
    return this.searchMode === 'train' ? this.trainDisplayedColumns : this.flightDisplayedColumns;
  }

  onBook(result: TrainSearchResultDto | FlightSearchResultDto) {
    // TODO: Navigate to booking module
    console.log('Booking:', result);
    // This will be implemented when booking module is created
    // Example: this.router.navigate(['/booking', this.searchMode, result.scheduleID]);
  }

  formatTime(dateTime: string): string {
    return new Date(dateTime).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      day: '2-digit',
      month: 'short'
    });
  }

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins}m`;
  }

  getAvailabilityClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'available':
        return 'text-green-600 bg-green-100';
      case 'limited':
        return 'text-yellow-600 bg-yellow-100';
      case 'waitlist':
        return 'text-red-600 bg-red-100';
      case 'sold out':
        return 'text-gray-600 bg-gray-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  }

  isTrainResult(result: any): result is TrainSearchResultDto {
    return 'trainNumber' in result;
  }

  isFlightResult(result: any): result is FlightSearchResultDto {
    return 'flightNumber' in result;
  }
}