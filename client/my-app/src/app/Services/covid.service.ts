import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Covid } from '../Models/Covid.model';

@Injectable({
  providedIn: 'root'
})
export class CovidService {

  public covidChartList = new Array();
  private hubConnection!: signalR.HubConnection;
  constructor() { }

  private startInvoke() {
    this.hubConnection.invoke('GetCovidList')
    .catch((error) => console.log(error))
  }

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44318/CovidHub").build();

    this.hubConnection.start()
    .then(() => 
    
    this.startInvoke()
    )
    .catch((err) => console.log(err))
  }

  startListener() {
    this.hubConnection.on('ReceiveCovidList', (covidCharts:Covid[]) =>
    {
      this.covidChartList = [];
      covidCharts.forEach((item) => {
        this.covidChartList.push([
          item.covidDate,
          item.counts[0],
          item.counts[1],
          item.counts[2],
          item.counts[3],
          item.counts[4],
        ])
      });
    }
    )
  }
}
