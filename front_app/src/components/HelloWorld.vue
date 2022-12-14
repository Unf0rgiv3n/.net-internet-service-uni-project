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
    <canvas
        class="chart--canvas"
        id="myChart"
    ></canvas>
  </v-container>
</template>

<script>
  import axios from 'axios'
  import Chart from 'chart.js/auto';
  
  const chartAreaBorder = {
    id: 'chartAreaBorder',
    beforeDraw(chart, args, options) {
      const { ctx, chartArea: { left, top, width, height } } = chart;
      ctx.save();
      ctx.strokeStyle = options.borderColor;
      ctx.lineWidth = options.borderWidth;
      ctx.setLineDash(options.borderDash || []);
      ctx.lineDashOffset = options.borderDashOffset;
      ctx.strokeRect(left, top, width, height);
      ctx.restore();
    },
  };
  
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
            filter: value => {
              if (!this.measurementFilter) return true

              return value < parseInt(this.measurementFilter)
            },
          },
          { 
            text: 'Unit', 
            value: 'unit' 
          },
        ],
        values: [],
        sensorTypeFilter:'',
        measurementFilter:'',
        types: ['INSOLATION', 'NOISE', 'TEMPERATURE', 'HUMIDITY', ''],
      }
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
            return x.value < parseInt(this.measurementFilter)
          })
        }
        a.forEach((row) => {
          csv += row.type
          csv += ','
          csv += row.name
          csv += ','
          csv += row.value
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
      },
      generatePlot() {
        if (this.sensorTypeFilter) {
          let stamps_o = this.values.filter((x)=>{return x.type === this.sensorTypeFilter})
          let stamps = stamps_o.map(s => s.value)
          let ctx = document.getElementById('myChart').getContext('2d');
          this.chart = new Chart(ctx, {
            type: 'line',
            data: {
              datasets: [
                {
                  color: '#fff',
                  label: this.sensorTypeFilter,
                  data: stamps,
                  backgroundColor: '#003183',
                  borderWidth: 2,
                },
              ],
              labels: [...Array(stamps.length).keys()],
            },
            options: {
              responsive: true,
              maintainAspectRatio: false,
              plugins: {
                chartAreaBorder: {
                  borderColor: 'white',
                  borderWidth: 0,
                  borderDash: [0, 0],
                  borderDashOffset: 2,
                },
              },
            },
            plugins: [chartAreaBorder],
          });
          this.chart.resize(0, window.innerHeight - 64 - 24 - 161 - 16 - 24);
        }
      }
    },
    async created(){
      console.log(await (await axios.get("http://localhost:18015/sensor")).data);
      this.values = await (await axios.get("http://localhost:18015/sensor")).data;
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