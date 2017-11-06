<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>项目填报</el-breadcrumb-item>
        <el-breadcrumb-item>荣誉填报</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
 <!-- 下方主内容 -->
  <el-col :span="24" class="warp-main left-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <el-form :inline="true" style="margin-bottom:15px">
        <el-button type="primary" @click="addFormVisible = true" >新增荣誉</el-button>
      </el-form>
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table  :data="HnrData" border style="width:100%" v-loading="listLoading" > 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="65" label="序号" style="text-aligin:center" align="center"></el-table-column>
        <el-table-column prop="AccountID" label="账号" sortable align="center" ></el-table-column>
        <el-table-column prop="AccountName" label="姓名" sortable align="center" ></el-table-column>
        <el-table-column prop="OrgName" label="所属单位" sortable align="center" ></el-table-column>
        <el-table-column prop="RoleName" label="角色" sortable align="center" ></el-table-column>
        <el-table-column label="操作" width="280" align="center">
          <template scope="scope">
            <el-button  size="small" @click="showModifyDialog(scope.$index,scope.row)" >编辑</el-button>
            <el-button type="success" size="small"  @click="resetAccTch(scope.$index,scope.row)" >重置</el-button>
            <el-button type="primary" size="small"  @click="blockAccAdm(scope.$index,scope.row)" >冻结</el-button>
            <el-button type="danger" size="small"  @click="delectAccTch(scope.$index,scope.row)" >删除</el-button>
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

    <!-- 新增表单 -->
    <el-dialog title="新增荣誉记录" :visible.sync="addFormVisible" v-loading="submitLoading" style="top:-11%" >
      <el-form :model="addFormBody" label-width="90px" ref="addForm" :rules="rules" auto  >
        <el-form-item label="荣誉项目" prop="HonorID">
          <el-select v-model="addFormBody.HonorID" placeholder="请选择荣誉" style="width:300px">
            <el-option v-for="honor in HonorData" :key="honor.HonorID" :value="honor.HonorID" :label="honor.Name"></el-option>
          </el-select>
        </el-form-item>  
        <el-form-item label="获奖年度" prop="Annual" >
          <el-select v-model="addFormBody.Annual"  placeholder="请选择年度" style="width:300px">
            <el-option v-for="options in annualOptions" :key="options.value" :label="options.label" :value="options.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="获奖日期" prop="HnrDate">
          <el-date-picker v-model="addFormBody.HnrDate" type="month" placeholder="获得年月" style="width:300px"></el-date-picker>
        </el-form-item>
        <el-form-item label="获奖人学号" prop="AwdeeID">
          <el-input v-model="addFormBody.AwdeeID" placeholder="请输入获奖人学号" style="width:300px" ></el-input>
        </el-form-item>          
        <el-form-item label="获奖人姓名" prop="AwdeeName">
          <el-input v-model="addFormBody.AwdeeName" placeholder="请输入获奖人姓名" style="width:300px" ></el-input>
        </el-form-item> 
        <el-form-item label="单位学院" prop="OrgID">
          <el-select v-model="addFormBody.OrgID" placeholder="请选择所属学院" style="width:300px">
            <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
          </el-select>
        </el-form-item> 
        <el-form-item label="所属团支部" prop="Branch">
          <el-input v-model="addFormBody.Branch" placeholder="请输入团支部" style="width:300px" ></el-input>
        </el-form-item>  
        <el-form-item label="上传图片" prop="FileName">
          <el-upload action="http://localhost:59996/"  :on-preview="handlePreview">
            <el-button size="small" type="primary">点击上传</el-button>
            <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div>
          </el-upload>
        </el-form-item>          
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" addFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="addSubmit" >提交</el-button>
      </div>
    </el-dialog>
</el-row>

</template>

<script type="text/ecmascript-6">
import {reqGetHonorList,reqGetOrgList} from '../../api/api'
import uptoken from '../../common/create_uptoken'
 export default {
   data() {
     return {
       // 填充荣明与数据
       HonorData:[],
       // 填充组织单位
       OrgData:[],
       // 用户令牌
       access_token:'',
       // 表格数据
       HnrData: [],
       listLoading:false,

       selectRowIndex:'',
       totalNum:0,
       page:1,
       size:10,
      // 新增表单相关数据
       submitLoading:false,       
       addFormVisible: false,
       addFormBody:{
         HonorId:'',
         Annual:'',
         HnrDate:'',
         AwdeeName:'',
         AwdeeID:'',
         OrgID:'',
         Branch:'',
         FileName:''
       },
      // 表单验证规则
      rules:{

      },
      // 获奖年度选择
      annualOptions:[
        {
          value:'2013-2014',
          label:'2013-2014'
        },
        {
          value:'2014-2015',
          label:'2014-2015'
        },
        {
          value:'2015-2016',
          label:'2015-2016'
        },                
      ]
     }    
   },
   //声明周期调用
   mounted(){
     this.getList();     
     this.getHonor();
     this.getOrg();
     console.log(this.OrgData)
   },
   methods:{
     // 填充荣誉数据
     getHonor(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetHonorList(param).then((res)=>{
         this.listLoading=false
         this.HonorData=res.data.data.list
                   console.log(this.HonorData)
       }).catch((res)=>{
         console.log(res)
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
     // 获取列表
     getList(){

     },

     // 上传图片成功钩子
     handlePreview(file){
       console.log(file)
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

</style>
