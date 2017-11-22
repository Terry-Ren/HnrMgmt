<template>
  <div class="container">
    <!-- 面包屑导航 -->      
    <div class="warp-breadcrum">
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>统计查询</el-breadcrumb-item>
          <el-breadcrumb-item>数据统计</el-breadcrumb-item>
      </el-breadcrumb>             
    </div>
    <!-- 下方主内容 --> 
    <div class="warp-body">
      <!-- 工具栏 -->    
      <div class="toolbal">      
      </div>
      <!-- 表格区 --> 
      <div class="main-data">  
        <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
        <div id="main" style="width: 600px;height:400px;"></div>            
      </div>
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
import * as echarts from "echarts";
export default {
  data() {
    return {
      // 指定图表的配置项和数据
      option: {
        title: {
          text: "ECharts 入门示例"
        },
        tooltip: {},
        legend: {
          data: ["销量"]
        },
        xAxis: {
          data: ["衬衫", "羊毛衫", "雪纺衫", "裤子", "高跟鞋", "袜子"]
        },
        yAxis: {},
        series: [
          {
            name: "销量",
            type: "bar",
            data: [5, 20, 36, 10, 10, 20]
          }
        ]
      }
    };
  },
  components: {},
  mounted(){
    this.created()
  },
  methods:{
    created(){
         var myChart = echarts.init(document.getElementById('main'));
                 // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(this.option);     
    }
  }
};
</script>

<style scoped lang="scss">

</style>
