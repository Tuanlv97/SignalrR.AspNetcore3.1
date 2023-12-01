import { Component, OnInit } from '@angular/core';
import { CovidService } from './Services/covid.service';
import { ChartType } from './types/chart-type';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{

  public title = 'SignalRAngular';
  public chartType = ChartType.LineChart;
   
  public columnNames = ['Tarih','Istanbul', 'Ankara', 'Izmir', 'Konya', 'Antalya'];
  public options: any = {legend: {position: 'Bottom'}};
  constructor(public covidService: CovidService) {}
  ngOnInit(): void {
    this.covidService.startConnection();
    this.covidService.startListener();
  }
}
