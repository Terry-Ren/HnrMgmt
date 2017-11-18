<template>
  <div class="container">
    <!-- 面包屑导航 -->      
    <div class="warp-breadcrum">
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>基础数据</el-breadcrumb-item>
        <el-breadcrumb-item>单位学院管理</el-breadcrumb-item>
      </el-breadcrumb>          
    </div>
    <!-- 下方主内容 --> 
    <div class="warp-body">
      <!-- 工具栏 -->    
      <div class="toolbal"> 
        <el-form :inline="true" style="margin-bottom:15px">
          <el-button type="primary" @click="addFormVisible = true" >新增单位</el-button>
        </el-form>             
      </div>
      <!-- 表格区 --> 
      <div class="main-data"> 
        <el-table class="table" :data="OrgData"  style="width:100%" height="string" v-loading="listLoading" > 
          <el-table-column type="selection" width="55"></el-table-column>
          <el-table-column type="index" width="65" label="序号" style="text-aligin:center" align="center"></el-table-column>
          <el-table-column prop="Name" label="单位名称" sortable align="center" ></el-table-column>
          <el-table-column label="操作" width="150">
            <template  slot-scope="scope">
              <el-button  size="small" @click="showModifyDialog(scope.$index,scope.row)" >编辑</el-button>
              <el-button type="danger" size="small"  @click="delectHornor(scope.$index,scope.row)" >删除</el-button>
            </template>
          </el-table-column>
        </el-table>           
      </div>
      <el-pagination layout="total, prev, pager, next, sizes, jumper" @size-change="SizeChangeEvent" @current-change="CurrentChangeEvent" :page-size="size" :page-sizes="[10,15,20,25,30]":total="totalNum">
      </el-pagination>      
    </div>
     <!-- 新增表单 -->
    <el-dialog title="新增单位信息" :visible.sync="addFormVisible" v-loading="submitLoading" >
      <el-form :model="addFormBody" label-width="80px" ref="addForm" :rules="rules" auto>
        <el-form-item label="单位名称" prop="Name">
          <el-input v-model="addFormBody.Name" placeholder="请输入单位名称"  ></el-input>
        </el-form-item>       
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" addFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="addSubmit" >提交</el-button>
      </div>
    </el-dialog>

    <!-- 编辑表单 -->
    <el-dialog title="编辑单位信息" :visible.sync="modifyFormVisible" v-loading="modifyLoading">
      <el-form :model="modifyFromBody" label-width="80px" ref="modifyFrom" :rules="rules" >
        <el-form-item label="单位名称" prop="Name"  >
          <el-input v-model="modifyFromBody.Name" placeholder="请输入单位名称"  ></el-input>
        </el-form-item>    
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" modifyFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="modifySubmit" >提交</el-button>
      </div>     
    </el-dialog>   
  </div>
</template>

<script type="text/ecmascript-6">
import {reqGetOrgList,posAddOrg,posModifyOrg,reqDeleteOrg} from '../../api/api'
import PubMethod from '../../common/util'
 export default {
   data() {
     return {
       // 用户令牌
       access_token:'',
       // 表格数据
       OrgData: [],
       listLoading:false,

       selectRowIndex:'',
       totalNum:0,
       page:1,
       size:10,

       // 表单规则验证
       rules:{
         Name:{
           required: true, message: '请输入单位名称' , trigger: 'blur' 
         },
       },

      // 新增表单相关数据
       submitLoading:false,       
       addFormVisible: false,
       addFormBody:{
         Name:'',
       },
       //编辑表单相关数据
       modifyFormVisible:false,
       modifyLoading:false,
       modifyFromBody:{
         Name:'',
       }


     }

   },
   //声明周期调用
   mounted(){
     this.getList();
   },

   //方法集合
   methods:{
     //获取荣誉列表
     getList(){
       this.listLoading=true
       let param={
           access_token : "11",
           page : this.page,
           limit : this.size
           }
           reqGetOrgList(param).then((res)=>{
               this.OrgData = res.data.data.list
               this.totalNum = res.data.data.count;
               //console.log(this.OrgData)
               this.listLoading=false
               }).catch((res)=>{
                   console.log(res)
                   })
            },
     //新增单位
     addSubmit(){
       let param
       this.$refs['addForm'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           //复制字符串
           let para = Object.assign({}, this.addFormBody);
           para.access_token='terry'
           posAddOrg(para).then((res)=>{
              this.submitLoading=false
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
              this.$refs['addForm'].resetFields();
              this.addFormVisible = false;
              this.getList();
           })           
         }
       })
     },
     //显示编辑
     showModifyDialog (index,row) {
       this.modifyFormVisible=true
       this.modifyFromBody= Object.assign({},row)
       this.selectRowIndex=index
       //console.log(this.selectRowIndex)
       },
    //保存编辑
    modifySubmit(){
      this.$refs['modifyFrom'].validate((valid)=>{
        if(valid){
          this.modifyLoading=true;
          let para = Object.assign({},this.modifyFromBody)
          para.access_token='terry'

          posModifyOrg(para).then((res)=>{
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
              this.$refs['modifyFrom'].resetFields()
              this.modifyFormVisible=false
              this.modifyLoading=false
              this.getList()
          })
        }
      })
    },
    //删除单位
    delectHornor(index,row){
      this.$confirm('此操作将永久删除该单位项, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          let para={orgID:row.OrgID}
          para.access_token='terry'
         
          reqDeleteOrg(para).then((res)=>{
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
            this.getList()
          })

            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消删除'
                });          
              });
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
    .el-pagination{
        text-align: right;
    }
</style>
