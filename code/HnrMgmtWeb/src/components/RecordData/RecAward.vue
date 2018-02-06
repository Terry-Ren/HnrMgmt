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
          <el-button type="primary" @click="review" :disabled="selectDisable" >审核</el-button>  
          <el-button type="primary" @click="rejectFormVisible=true;" :disabled="selectDisable" >驳回</el-button>                      
        </el-form>
      </div>
        <div class="main-data">
        <el-table class="table" :data="AwdData"   v-loading="listLoading" height="string" @select="selectRow"  > 
          <el-table-column type="selection"></el-table-column>
          <el-table-column type="index"  label="序号" style="text-aligin:center" align="center"></el-table-column>
          <el-table-column prop="AwdName" label="奖项名称" sortable align="center" ></el-table-column>
          <el-table-column prop="AwdYear" label="获得年度" sortable align="center" ></el-table-column>
          <el-table-column prop="Grade" label="等次" sortable align="center" :formatter="transfGrande"></el-table-column>
          <el-table-column prop="GradeName" label="级别" sortable align="center" :formatter="transfGrandeName"></el-table-column>        
          <el-table-column prop="AwdProName" label="项目名称" sortable align="center" ></el-table-column>        
          <el-table-column prop="AwdOrgName" label="所属学院" sortable align="center" ></el-table-column>
          <el-table-column prop="AwdeeName" label="主要学生" sortable align="center" ></el-table-column>
          <el-table-column prop="State" label="审核状态"  align="center"  :filters="StateArray" :filter-method="filterState" >
            <template slot-scope="scope">
              <el-tag :type="stateTag(scope.$index,scope.row)">{{transfRecordState(scope.row)}}</el-tag>
            </template>
          </el-table-column>               
          <el-table-column label="操作" width="280" align="center" >
            <template slot-scope="scope">
              <el-button  size="small" @click="switchDetial(scope.$index,scope.row)" >详情</el-button>             
              <el-button type="success" size="small"  @click="switchModify(scope.$index,scope.row)" >重填</el-button>
              <el-button type="danger" size="small"  @click="delectRecord(scope.$index,scope.row)" >删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        </div>
      <!-- 下方工具条 -->
        <el-pagination layout="total, prev, pager, next, sizes, jumper" @size-change="SizeChangeEvent" @current-change="CurrentChangeEvent" :page-size="size" :page-sizes="[10,15,20,25,30]" :total="totalNum">
        </el-pagination>
    </div> 
    <!-- 驳回请求表单      -->
    <el-dialog title="驳回请求" :visible.sync="rejectFormVisible" v-loading="rejectLoading" style="width:70%;">
      <el-form  label-width="80px" ref="rejectFrom"  >
        <el-form-item label="驳回理由" prop="reson"  >
          <el-input v-model="RejectReason"  type="textarea" :autosize="{ minRows: 2, maxRows: 4}" placeholder="请输入驳回理由" style="width:300px" ></el-input>
        </el-form-item>  
      </el-form> 
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" rejectFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="rejectSubmit" >提交</el-button>
      </div>            
    </el-dialog> 
  </div>
</template>

<script type="text/ecmascript-6">
import {reqGetRecord,reqDeleteRecord,reqGetReviewRecord,reqGetRejectRecord} from '../../api/api'
import PubMethod from '../../common/util'
import * as types from "../../store/mutation-types";
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
       // 用户令牌
       access_token:'',
       // 表格数据
       AwdData: [],
       listLoading:false,
       //审核状态筛选
       StateArray:[
         {text:'待审核',value:'0'},
         {text:'院审通过',value:'1'},         
         {text:'校审通过',value:'2'},
         {text:'已驳回',value:'3'}
       ],
       //选择表格区域与审核
       selectRowData:[],
       selectDisable:true,
       rejectFormVisible:false,
       rejectLoading:false,
       RejectReason:'',
       //翻页获取
       totalNum:0,
       page:1,
       size:10,
     }    
   },
   watch:{
     selectRowData(newVal){
       if(newVal.length==0)
       this.selectDisable=true
     }
   },
  // 计算属性
  computed: {
    recordModify: function() {
      return this.$store.state.recordModify;
    }
  },
  // 监听者
  watch: {
    recordModify: {
      handler: function(params) {
        this.getList();
      }
    },
     selectRowData(newVal){
       if(newVal.length==0)
       this.selectDisable=true
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
         path:'/record/addaward'
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
     // 筛选审核状态
     filterState(value,row){

       return row.State==value
     } ,  
     // 审核值样式
     stateTag(index,row){
       return row.State=='0' ? 'info' : row.State=='1' ? ' ' : row.State=='2' ? 'success' : 'danger'
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
          console.log(this.AwdData)
          this.listLoading=false
       }).catch((res)=>{
         console.log(res)
       })
     },
     //  显示详情页面
     switchDetial (index,row) {
      this.$store.commit(types.RECORD_AWARD, row);
      this.$router.push({
        path: "/record/award/detail"
      });
       },
    //  显示编辑页面
    switchModify(index, row) {
      //  this.ismodify='detail'
      this.$store.commit(types.RECORD_AWARD, row);
      this.$router.push({
        path: "/record/award/modify"
      });
    },  
    //删除功能
    delectRecord(index, row) {
      this.$confirm("此操作将永久删除该记录, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          let para = { AwdRecordID: row.AwdRecordID };
          para.access_token = "11";

          reqDeleteRecord(para).then(res => {
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this, res.data);
            this.getList();
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除"
          });
        });
    },      
    // 选中单一行
    selectRow(selection, row){
      this.selectDisable=false
      this.selectRowData=[]
      selection.forEach(rowData => {
        this.selectRowData.push(rowData.AwdRecordID) 
      });      
      //console.log(this.selectRowData)
    } ,    
    // 审核通过  
    review(){
      this.$confirm("是否审核通过该记录?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {     
          this.listLoading=true
          let param={
            access_token:'11',
            AwdRecordID:this.selectRowData[0]
          }
          reqGetReviewRecord(param).then(res=>{
            this.listLoading=false
            PubMethod.statusinfo(this,res.data)
            this.getList();
          })
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除"
          });
        });
    },
    // 驳回
    rejectSubmit(){
      //this.$refs["rejectFrom"].validate(valid => {
       // if (valid) {
          this.rejectLoading = true;
          //复制字符串
          let param={
            access_token:'11',
            AwdRecordID:this.selectRowData[0],
            RejectReason:this.RejectReason
          }
          reqGetRejectRecord(param).then(res => {
            this.rejectLoading = false;
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this, res.data);
            this.$refs["rejectFrom"].resetFields();
            this.rejectFormVisible = false;
            this.getList();
          });
      //  }
     // });      
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
.el-dialog__wrapper{
  right: 25vw;
  position: absolute;
}
</style>
