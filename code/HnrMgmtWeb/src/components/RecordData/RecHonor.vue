<template>
  <div class="container">
    <div class="warp-breadcrum">
      <!-- 面包屑导航 -->
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>项目填报</el-breadcrumb-item>
          <el-breadcrumb-item>荣誉填报</el-breadcrumb-item>
        </el-breadcrumb>      
    </div>
    <!-- 下方主内容 -->
    <div class="warp-body">
      <!-- 工具栏 -->
      <div class="toolbal">
        <el-form :inline="true" style="margin-bottom:15px">
          <el-button type="primary" @click="toAddFrom" >新增荣誉</el-button>
        </el-form>        
      </div>
      <div class="main-data">
        <!-- 表格区 -->
          <el-table class="table" :data="HnrData"  style="width:100%" v-loading="listLoading" height="string" > 
            <el-table-column type="selection" ></el-table-column>
            <el-table-column type="index"  label="序号" style="text-aligin:center" align="center"></el-table-column>
            <el-table-column prop="HnrName" label="荣誉名称" sortable align="center" ></el-table-column>
            <el-table-column prop="HnrAnnual" label="获得年度" sortable align="center" ></el-table-column>
            <el-table-column prop="HnrGradeName" label="级别" sortable align="center" :formatter="transfGrandeName" ></el-table-column>
            <el-table-column prop="AwardeeName" label="姓名" sortable align="center" ></el-table-column>
            <el-table-column prop="AwardeeOrgName" label="单位学院" sortable align="center" ></el-table-column>
            <el-table-column prop="State" label="审核状态" sortable align="center" :formatter="transfRecordState" ></el-table-column>        
            <el-table-column label="操作" width="280" align="center">
              <template slot-scope="scope" >
                <el-button  size="small" @click="switchDetial(scope.$index,scope.row)" >详情</el-button>
                <el-button type="success" size="small"  @click="resetAccTch(scope.$index,scope.row)" >重填</el-button>
                <el-button type="danger" size="small"  @click="delectAccTch(scope.$index,scope.row)" >删除</el-button>
              </template>
            </el-table-column>
          </el-table>          
      </div>
      <!-- 下方工具条 -->      
      <el-pagination layout="total, prev, pager, next, sizes, jumper" @size-change="SizeChangeEvent" @current-change="CurrentChangeEvent" :page-size="size" :page-sizes="[10,15,20,25,30]":total="totalNum">
      </el-pagination>      
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
import {posRecordHonor,reqGetRecord} from '../../api/api'
import PubMethod from '../../common/util'
import * as types from '../../store/mutation-types'
// import uptoken from '../../common/create_uptoken'
 export default {
   data() {
     // 获奖人学号验证规则
     var validateAwdeeID =(rule, value, callback) => {
       const RULES =/^\d{5,13}$/
       if (value == null) {
         callback(new Error('学号不能为空'))
         } else if(!RULES.test(value)){
           callback(new Error('必须为5-13位数字'))
           }
           else{
             callback()
           }
      }
     return {
       // 七牛云令牌
       postData:{
         token:this.$store.state.uploadToken
       },
       // 填充荣誉数据
       HonorData:[],
       // 填充组织单位
       OrgData:[],
       // 用户令牌
       access_token:'',
       // 表格数据
       HnrData: [],
       listLoading:false,
       // 分页信息
       totalNum:0,
       page:1,
       size:10,
     }    
   },
   //声明周期调用
   mounted(){
     this.getList();     
   },
   methods:{
     // 跳转路由
     toAddFrom(){
       this.$router.push({
         path:'/record/addhonor'
       })
     },
     // 获取列表
     getList(){
       this.listLoading=true
       let param={
         page : this.page,
         limit : this.size,
         type:1,
         access_token:"11"
       }
       reqGetRecord(param).then((res)=>{
          this.HnrData = res.data.data.hnrList
          this.totalNum = res.data.data.hnrListNum;
          //console.log(this.HnrData)
          this.listLoading=false
       }).catch((res)=>{
         console.log(res)
       })
     },
     // 荣誉级别转换
     transfGrandeName(row){
       return PubMethod.transfGrandeName(row) 
     },
     // 审核状态转换
     transfRecordState(row){
       return PubMethod.transfRecordState(row)
     },
     //  显示详情页面
     switchDetial (index,row) {
      this.$store.commit(types.RECORD_HONOR,row)
      this.$router.push({
        path:'/record/honor/modify'
      })
       },     
    //更换每页数量
    SizeChangeEvent(val){
        this.loading=true;
        this.size = val;
        this.getList();
        PubMethod.logMessage(this.page + "   " + this.size);
    },
    //页码切换时
    CurrentChangeEvent(val){
        this.loading=true;
        this.page = val;
        this.getList();
        PubMethod.logMessage(this.page + "   " + this.size);
    }
   }
 }
</script>

<style scoped lang="scss">
.left-main{
  border-radius: 5px;
}
.hornor-add{
  margin-left: 120px
}

</style>
