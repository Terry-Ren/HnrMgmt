<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>基础数据</el-breadcrumb-item>
        <el-breadcrumb-item>奖项管理</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
  <!-- 下方主内容 -->
  <el-col :span="24" class="warp-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <el-form :inline="true">
        <el-button type="primary" @click="getUser">新增奖项</el-button>
      </el-form>
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table :data="AwdData" border style="width:100%" v-loading="loading" > 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="60"></el-table-column>
        <el-table-column prop="Name" label="奖项名称" sortable></el-table-column>
        <el-table-column prop="GradeName" label="奖项级别" sortable></el-table-column>
        <el-table-column prop="Grade" label="获奖等次" sortable></el-table-column>
        <el-table-column label="操作" width="150">
          <template scope="scope">
            <el-button  size="small" @click="AwdEdit(scope.$index, scope.row)">编辑</el-button>
            <el-button type="danger" size="small" @click="AwdDelete(scope.$index,scope.row)" >删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-col>
    <!-- 下方工具条 -->
    <el-col :span="24">
      <el-pagination layout="total, prev, pager, next, sizes, jumper" @size-change="SizeChangeEvent" @current-change="CurrentChangeEvent" :page-size="size" :page-sizes="[10,15,20,25,30]":total="totalNum">
      </el-pagination>
    </el-col>
  </el-col>
</el-row>
</template>

<script type="text/ecmascript-6">
import {reqGetAwdListPage} from '../../api/api'
import PubMethod from '../../common/public'

 export default {
   data() {
     return {
       AwdData: [],
       totalNum:0,
       page:1,
       size:10,
       loading : true
     }
   },
   mounted(){
     this.getList();
   },
   methods:{
    logInfo(message){
        PubMethod.logMessage(message);
    },
     //获取荣誉列表
    getList(){
       let param = {
            access_token : "11",
            page : this.page,
            limit : this.size
        }
        reqGetAwdListPage(param).then((res)=>{
            this.AwdData = res.data.data.list;
            this.totalNum = res.data.data.count;
            this.loading = false;
        }).catch*((res)=>{
            PubMethod.logMessage(res)
        })
    },
    reGetList(){
       let param = {
            access_token : "11",
            page : this.page,
            limit : this.size
        }
        reqGetAwdListPage(param).then((res)=>{
            this.AwdData = res.data.data.list;
            this.loading = false;
            //this.totalNum = res.data.data.count;
        }).catch*((res)=>{
            PubMethod.logMessage(res)
        })
    },
    getUser(){
         PubMethod.logMessage("getUser");
     },
    AwdEdit(a,b){
        PubMethod.logMessage(a);
        PubMethod.logMessage(b);
    },
    AwdDelete(a,b){
        PubMethod.logMessage(a);
        PubMethod.logMessage(b);
    },
    SizeChangeEvent(val){
        this.loading=true;
        this.size = val;
        this.reGetList();
        PubMethod.logMessage(this.page + "   " + this.size);
    },
    CurrentChangeEvent(val){
        this.loading=true;
        this.page = val;
        this.reGetList();
        PubMethod.logMessage(this.page + "   " + this.size);
    }
   }
 }
</script>

