<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>基础数据</el-breadcrumb-item>
        <el-breadcrumb-item>荣誉项目管理</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
  <!-- 下方主内容 -->
  <el-col :span="24" class="warp-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <el-form :inline="true">
        <el-button type="primary" @click="getUser">新增荣誉</el-button>
      </el-form>
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table  :data="HnrData" border style="width:100%" > 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="60"></el-table-column>
        <el-table-column prop="customerId" label="荣誉名称" sortable></el-table-column>
        <el-table-column prop="companyName" label="荣誉级别" sortable></el-table-column>
        <el-table-column label="操作" width="150">
          <template scope="scope">
            <el-button  size="small" @click="handleEdit(scope.$index, scope.row)">编辑</el-button>
            <el-button type="danger" size="small" @click="delBook(scope.$index,scope.row)" >删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-col>
    <!-- 下方工具条 -->
    <el-col :span="24">
      <el-pagination   small layout="prev, pager, next" :total="20">
  </el-pagination>
    </el-col>
  </el-col>
</el-row>
</template>

<script type="text/ecmascript-6">
import {reqGetBookListPage} from '../../api/api'
 export default {
   data() {
     return {
       HnrData: []
     }

   },
   mounted(){
     this.getList();
   },
   methods:{
     //获取荣誉列表
     getList(){
       let param;
       reqGetBookListPage(param).then((res)=>{
         console.log(res)
          this.HnrData = res.data;
       }).catch((res)=>{
         console.log(res)
       })
     }
   }
 }
</script>

<style scoped lang="scss">

 
</style>
