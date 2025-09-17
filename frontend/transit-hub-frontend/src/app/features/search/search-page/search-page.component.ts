import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SearchFormComponent } from '../search-form/search-form.component';
import { SearchResultsComponent } from '../search-results/search-results.component';
import { TrainSearchResultDto } from '../../../models/train-search.dto';
import { FlightSearchResultDto } from '../../../models/flight-search.dto';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, SearchFormComponent, SearchResultsComponent],
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.css']
})
export class SearchPageComponent {
  searchResults: (TrainSearchResultDto | FlightSearchResultDto)[] = [];
  isLoading = false;
  searchMode: 'train' | 'flight' = 'train';

  onSearchResults(results: TrainSearchResultDto[] | FlightSearchResultDto[]) {
    this.searchResults = results;
    this.isLoading = false;
  }

  onSearchModeChange(mode: 'train' | 'flight') {
    this.searchMode = mode;
    this.searchResults = []; // Clear previous results when mode changes
  }

  onSearchStart() {
    this.isLoading = true;
    this.searchResults = [];
  }
}