<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{path:'/'}">首页</el-breadcrumb-item>
        <el-breadcrumb-item>系统管理</el-breadcrumb-item>
        <el-breadcrumb-item>角色管理</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
<!-- 下方主内容 -->
  <el-col :span="24" class="warp-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <!-- <el-form :inline="true" style="margin-bottom:15px">
        <el-button type="primary" @click="addFormVisible = true" >新增角色</el-button>
      </el-form> -->
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table  :data="Roledata" border style="width:100%" v-loading="listLoading" > 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="65" label="序号" style="text-aligin:center" align="center"></el-table-column>
        <el-table-column prop="Name" label="角色名" sortable align="center" ></el-table-column>
        <el-table-column label="操作" width="200" sortable align="center">
          <template slot-scope="scope">
            <el-button  size="small" @click="showMgmtDialog(scope.$index,scope.row)"  >管理</el-button>
            <!-- <el-button type="danger" size="small" >删除</el-button> -->
          </template>
        </el-table-column>
      </el-table>
    </el-col>      
  </el-col>

    <!-- 新增表单 -->
    <!-- <el-dialog title="新增角色" :visible.sync="addFormVisible" v-loading="submitLoading" >
      <el-form :model="addFormBody" label-width="80px" ref="addForm"  auto>
        <el-form-item label="角色名" prop="Name">
          <el-input v-model="addFormBody.Name" placeholder="请输入账号"  ></el-input>
        </el-form-item>  
        <el-form-item label="姓名" prop="AccountName">
          <el-input v-model="addFormBody.AccountName" placeholder="请输入姓名"  ></el-input>
        </el-form-item>
        <el-form-item label="电话" prop="Tel">
          <el-input v-model="addFormBody.Tel" placeholder="请输入电话"  ></el-input>
        </el-form-item>            
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" addFormVisible = false">取消</el-button>
        <el-button type="primary"  >提交</el-button>
      </div>
    </el-dialog> -->

    <!-- 管理表单 -->
    <el-dialog title="角色权限管理" :visible.sync="mgmtFormVisible" v-loading="mgmtLoading">
      <el-form :model="mgmtFromBody" label-width="120px" ref="mgmtFrom"  >
        <el-form-item label="角色名" prop="Name" >
          <el-input v-model="mgmtFromBody.Name" placeholder="请输入角色名" :disabled="true" ></el-input>
        </el-form-item>
        <el-form-item label="已授权权限"  label-width="120px" >
            <el-checkbox-group v-model="initSelect">
              <el-checkbox  v-for="Control in selectRole" :key="Control.ApiID" :label="Control.ApiName" disabled ></el-checkbox>
            </el-checkbox-group>  
        </el-form-item>  
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" mgmtFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="mgmtSubmit" >提交</el-button>
      </div>     
    </el-dialog>
</el-row>
</template>

<script type="text/ecmascript-6">
import {reqGetRoleList,reqGetRoleControl} from '../../api/api'
 export default {
   data() {
     return {
       // 表格数据
       Roledata:[],
       listLoading:false,
       selectRowIndex:'',

      // 新增表单相关数据
       submitLoading:false,       
       addFormVisible: false,
       addFormBody:{
         AccountID:'',
         AccountName:'',
         RoleID:'',
         OrgID:'',
         Tel:''
       },

       //编辑表单相关数据
       mgmtFormVisible:false,
       mgmtLoading:false,
       mgmtFromBody:{
           Name:'',
           ControlId:'',
       },

       //权限控制
       ExAccessControl:[],
       selectRole:[],
       initSelect:['新增荣誉称号']
     }

   },
   //声明周期调用
   mounted(){
     this.getList();
     this.getRoleControl();
   },

   // 方法集合
   methods: {
       //获取所有角色对应权限
       getRoleControl(){
           this.listLoading=true;
           let param={
               access_token : "11",           
           }
           reqGetRoleControl(param).then((res)=>{
               this.ExAccessControl=res.data.data.list
                //console.log(this.ExAccessControl[1].MenuList)
               this.listLoading=false
           }).catch((res)=>{
               console.log(res)
           })
       },
     //获取角色列表
     getList(){
       this.listLoading=true
       let param={
           access_token : "11",
           }
           reqGetRoleList(param).then((res)=>{
               this.Roledata = res.data.data.list
               //console.log(this.AccData)
               this.listLoading=false
               }).catch((res)=>{
                   console.log(res)
                   })
            },
    // 显示管理界面
     showMgmtDialog(index,row){
       this.mgmtFormVisible=true
       this.mgmtFromBody= Object.assign({},row)
       this.selectRole=this.ExAccessControl[index].MenuList
       // console.log(this.selectRole)
       this.selectRowIndex=index
     }
   }
 }
</script>

<style scoped lang="scss">

 
</style>
