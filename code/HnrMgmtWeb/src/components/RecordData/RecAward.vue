<template>
  <div class="container">
    <div class="warp-breadcrum">
      <!-- 面包屑导航 -->
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>项目填报</el-breadcrumb-item>
          <el-breadcrumb-item>奖项填报</el-breadcrumb-item>
      </el-breadcrumb>      
    </div>
    <div class="warp-body">
      <!-- 表格区 -->
      <!-- 工具栏 -->
      <div class="toolbal">
        <el-form :inline="true" style="margin-bottom:15px">
          <el-button type="primary" @click="toAddFrom" >新增奖项</el-button>
        </el-form>
      </div>
        <div class="main-data">
        <el-table class="table" :data="AwdData"   v-loading="listLoading" height="string"  > 
          <el-table-column type="selection"></el-table-column>
          <el-table-column type="index"  label="序号" style="text-aligin:center" align="center"></el-table-column>
          <el-table-column prop="AwdName" label="奖项名称" sortable align="center" ></el-table-column>
          <el-table-column prop="AwdYear" label="获得年度" sortable align="center" ></el-table-column>
          <el-table-column prop="Grade" label="等次" sortable align="center" :formatter="transfGrande"></el-table-column>
          <el-table-column prop="GradeName" label="级别" sortable align="center" :formatter="transfGrandeName"></el-table-column>        
          <el-table-column prop="AwdProName" label="项目名称" sortable align="center" ></el-table-column>        
          <el-table-column prop="AwdOrgName" label="所属学院" sortable align="center" ></el-table-column>
          <el-table-column prop="AwardeeName" label="主要学生" sortable align="center" ></el-table-column>
          <el-table-column prop="State" label="审核状态" sortable align="center" :formatter="transfRecordState" ></el-table-column>               
          <el-table-column label="操作" width="280" align="center" >
            <template slot-scope="scope">
              <el-button  size="small" @click="showDetialDialog(scope.$index,scope.row)" >详情</el-button>
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
    <!-- 详情表单 -->
    <el-dialog title="荣誉记录查看" :visible.sync="detailFormVisible" v-loading="detailLoading" width="70%" >
      <el-form :model="detailFormBody" label-width="80px" ref="detailFrom" :rules="rules" >
        <el-form-item label="项目名称" prop="ProjectName"  >
          <el-input v-model="detailFormBody.ProjectName" placeholder="请输入项目名称"  ></el-input>
        </el-form-item>    
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" detailFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="detailSubmit" >提交</el-button>
      </div>     
    </el-dialog>      

  </div>
</template>

<script type="text/ecmascript-6">
import {reqGetAwdList,reqGetOrgList,posRecordAward,reqGetRecord} from '../../api/api'
import PubMethod from '../../common/util'
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
       // 填充荣明与数据
       AwardData:[],
       // 填充组织单位
       OrgData:[],
       // 用户令牌
       access_token:'',
       // 表格数据
       AwdData: [],
       listLoading:false,

       selectRowIndex:'',
       totalNum:0,
       page:1,
       size:10,
      // 表单验证规则
      rules:{
        AwardID:{required:true , message:'请选择荣誉项目', trigger:'blur'},
        Year:{required:true , message:'请选择获得年度', trigger:'blur'},
        AwdTime:{required:true , message:'请选择获得年月', trigger:'blur'},
        AwdeeID:[
          {required:true , message:'请输入学号', trigger:'blur'},
          {validator:validateAwdeeID, tigger:'blure'},          
        ],
        AwdeeName:{required:true , message:'请输入获奖人姓名', trigger:'blur'},
        OrgID:{required:true , message:'请选择单位学院', trigger:'blur'},
        Branch:{required:true , message:'请输入所属团支部', trigger:'blur'}         
      },
      // 详情表单内容
      detailLoading:false,
      detailFormVisible:false,
      detailFormBody:{
         AwardID:'',
         Year:'',
         Term:'',
         AwdTime:'',
         ProjectName:'',
         IsTeam:'',
         Teacher:[],
         Members:[],
         OrgID:'',
         FileUrl:'-1'      
      }
     }    
   },
//    // 计算属性
//    computed:{
//        Branch(){
//            return this.addFormBody.Branch
//        }
//    },
//    // 观察着
//    watch:{
//        Branch(newVal){
//            this.addFormBody.Branch=newVal+'团支部'
//        }
//    },
   //声明周期调用
   mounted(){
     this.getList();     
     this.getHonor();
     this.getOrg();
   },
   methods:{
     // 跳转路由
     toAddFrom(){
       this.$router.push({
         path:'/record/addaward'
       })
     },
     // 填充荣誉数据
     getHonor(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetAwdList(param).then((res)=>{
         this.listLoading=false
         this.AwardData=res.data.data.list
              this.transfOptionGrande()
       }).catch((res)=>{
         console.log(res)
         })       
     },
     // 转换新增选项中的获奖级别
     transfOptionGrande(){
         this.AwardData.forEach((award)=>{
           //console.log(award)
             award.Grade=PubMethod.transfGrande(award)
         })

     },
    // 填充单位数据
     getOrg(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetOrgList(param).then((res)=>{
         this.listLoading=false
         this.OrgData=res.data.data.list
       }).catch((res)=>{
         console.log(res)
         })       
     },
     // 转换表格中获奖等次
     transfGrande(row){
       return PubMethod.transfGrande(row)
     },
     // 转换表格中的获奖级别
     transfGrandeName(row){
       return PubMethod.transfGrandeName(row) 
     },
     // 审核状态转换
     transfRecordState(row){
       return PubMethod.transfRecordState(row)
     },     
     // 获取列表
     getList(){
       this.listLoading=true
       let param={
         page : this.page,
         limit : this.size,
         access_token:"11",
         type:2,
       }
       reqGetRecord(param).then((res)=>{
          this.AwdData = res.data.data.awdList
          this.totalNum = res.data.data.awdListNum;
          //console.log(this.AwdData)
          this.listLoading=false
       }).catch((res)=>{
         console.log(res)
       })
     },
     //  显示详情页面
     showDetialDialog (index,row) {
       this.detailFormVisible=true
       this.detailFromBody= Object.assign({},row)
       this.selectRowIndex=index
       //console.log(this.selectRowIndex)
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
  border: 2px;
}
.hornor-add{
  margin-left: 120px
}

</style>
