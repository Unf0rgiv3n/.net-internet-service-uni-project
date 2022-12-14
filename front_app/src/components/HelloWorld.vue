<template>
  <v-container>
    <v-btn
        @click="generateCSV()">DOWNLOAD CSV
    </v-btn>
    <v-btn
        @click="generateJSON()">DOWNLOAD JSON
    </v-btn>
    <v-data-table
        :headers="headers"
        :items="values"
        :items-per-page="5"
        class="elevation-1"
    ></v-data-table>
    <v-text-field
        v-model="measurementFilter"
        type="number"
        label="Less than"
    ></v-text-field>
    <v-select
        v-model="sensorTypeFilter"
        :items="types"
        label="Select"
        persistent-hint
        single-line
    ></v-select>
    <v-col class="col-12">
      <canvas
          class="chart--canvas"
          id="myChart"
      ></canvas>  
    </v-col>
  </v-container>
</template>

<script>
  import axios from 'axios'
  import Chart from 'chart.js/auto';
  
  export default {
    name: 'HelloWorld',

   data() {
      return {
        headers: [
          {
            text: 'Sensor type',
            align: 'start',
            sortable: false,
            value: 'type',
            filter: value => {
              if (!this.sensorTypeFilter) return true

              return value === this.sensorTypeFilter
            },
          },
          { 
            text: 'Sensor ID', 
            value: 'name' 
          },
          { 
            text: 'Measurement', 
            value: 'measurement',
            filter: measurement => {
              if (!this.measurementFilter) return true

              return measurement < parseInt(this.measurementFilter)
            },
          },
          { 
            text: 'Unit', 
            value: 'unit' 
          },
        ],
        stamps: [],
        values: [],
        sensorTypeFilter:'',
        measurementFilter:'',
        types: ['INSOLATION', 'NOISE', 'TEMPERATURE', 'HUMIDITY', ''],
        chart: undefined,
      }
   }, 
    watch: {
      sensorTypeFilter: {
        handler() {
          this.stamps = this.values.filter((x)=>{return x.type === this.sensorTypeFilter}).map(s => s.measurement);
          console.log(this.stamps);
          this.chart.data.labels = [...Array(this.stamps.length).keys()];
          this.chart.data.datasets[0].data = this.stamps;
          this.chart.data.datasets[0].label = this.sensorTypeFilter;
          this.chart.update();
        },
        // force eager callback execution
        immediate: true
      },
    },
    methods: {
      generateCSV() {
        console.log("generate csv");
        let csv = 'Sensor type,Sensor ID,Measurement,Unit\n';
        let a = []
        if (this.sensorTypeFilter === '') {
          a = this.values
        }
        else{
          a = this.values.filter((x)=>{return x.type === this.sensorTypeFilter})
        }
        if (this.measurementFilter !== '') {
          a = a.filter((x) => {
            return parseInt(x.measurement) < parseInt(this.measurementFilter)
          })
        }
        a.forEach((row) => {
          csv += row.type
          csv += ','
          csv += row.name
          csv += ','
          csv += row.measurement
          csv += ','
          csv += row.unit
          csv += "\n";
        });
        const anchor = document.createElement('a');
        anchor.href = 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv);
        anchor.target = '_blank';
        anchor.download = 'sensors.csv';
        anchor.click();
      },
      generateJSON() {
        console.log("generate json");
        let table = []
        if (this.sensorTypeFilter === '') {
          table = this.values
        }
        else{
          table = this.values.filter((x)=>{return x.type === this.sensorTypeFilter})
        }
        if (this.measurementFilter !== '') {
          table = table.filter((x) => {
            return parseInt(x.measurement) < parseInt(this.measurementFilter)
          })
        }
        const data = JSON.stringify(table)
        const blob = new Blob([data], {type: 'text/plain'})
        const e = document.createEvent('MouseEvents'),
            a = document.createElement('a');
        a.download = "test.json";
        a.href = window.URL.createObjectURL(blob);
        a.dataset.downloadurl = ['text/json', a.download, a.href].join(':');
        e.initEvent('click', true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        a.dispatchEvent(e);
      },
    },
    async created(){
      this.values = await (await axios.get("http://localhost:18015/sensor")).data;
      this.stamps = this.values.filter((x)=>{return x.type === this.sensorTypeFilter}).map(s => s.measurement);
    },
    mounted(){
      const labels = ['1', '2', '3', '4', '5', '6', '7']
      const ctx = document.getElementById('myChart').getContext('2d');
      this.chart = new Chart(ctx, {
        type: 'line',
        data: {
          labels: labels,
          datasets: [{
            label: 'My First Dataset',
            data: [65, 59, 80, 81, 56, 55, 40],
            fill: false,
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
          }]
        }
      });
      this.chart.resize(0, window.innerHeight - 64 - 24 - 161 - 16 - 24);
    }
  }
</script>

<style>
.chart--canvas {
  background-color: #fff;
  padding: 20px;
  border-radius: 4px;
  margin-top: 16px;
}
</style>