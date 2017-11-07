<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>系统管理</el-breadcrumb-item>
        <el-breadcrumb-item>账户管理</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
 <!-- 下方主内容 -->
  <el-col :span="24" class="warp-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <el-form :inline="true" style="margin-bottom:15px">
        <el-button type="primary" @click="addFormVisible = true" >新增账户</el-button>
      </el-form>
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table  :data="AccData" border style="width:100%" v-loading="listLoading" > 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="65" label="序号" style="text-aligin:center" align="center"></el-table-column>
        <el-table-column prop="AccountID" label="账号" sortable align="center" ></el-table-column>
        <el-table-column prop="AccountName" label="姓名" sortable align="center" ></el-table-column>
        <el-table-column prop="OrgName" label="所属单位" sortable align="center" ></el-table-column>
        <el-table-column prop="RoleName" label="角色" sortable align="center" ></el-table-column>
        <el-table-column prop="State" label="状态" sortable align="center" :formatter="transfState" ></el-table-column>
        <el-table-column label="操作" width="280" align="center">
          <template slot-scope="scope">
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

    <!-- 新增表单 -->
    <el-dialog title="新增助理信息" :visible.sync="addFormVisible" v-loading="submitLoading" >
      <el-form :model="addFormBody" label-width="80px" ref="addForm" :rules="rules" auto>
        <el-form-item label="账号" prop="AccountID">
          <el-input v-model="addFormBody.AccountID" placeholder="请输入账号"  ></el-input>
        </el-form-item>  
        <el-form-item label="姓名" prop="AccountName">
          <el-input v-model="addFormBody.AccountName" placeholder="请输入姓名"  ></el-input>
        </el-form-item>
        <el-form-item label="电话" prop="Tel">
          <el-input v-model="addFormBody.Tel" placeholder="请输入电话"  ></el-input>
        </el-form-item>
        <el-form-item label="角色" prop="RoleID">
          <el-select v-model="addFormBody.RoleID" placeholder="请选择新增角色">
            <el-option v-for="role in RoleData" :key="role.RoleID" :value="role.RoleID" :label="role.Name"></el-option>
          </el-select>
        </el-form-item> 
        <el-form-item label="单位" prop="OrgID">
          <el-select v-model="addFormBody.OrgID" placeholder="请选择所属单位">
            <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
          </el-select>
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
        <el-form-item label="账号" prop="AccountID">
          <el-input v-model="modifyFromBody.AccountID" placeholder="请输入账号"  ></el-input>
        </el-form-item>  
        <el-form-item label="姓名" prop="AccountName">
          <el-input v-model="modifyFromBody.AccountName" placeholder="请输入姓名"  ></el-input>
        </el-form-item>
        <el-form-item label="电话" prop="Tel">
          <el-input v-model="modifyFromBody.Tel" placeholder="请输入电话"  ></el-input>
        </el-form-item>
        <el-form-item label="单位" prop="Tel">
          <el-select v-model="modifyFromBody.OrgID" placeholder="请选择所属单位">
            <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
          </el-select>
        </el-form-item>   
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" modifyFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="modifySubmit" >提交</el-button>
      </div>     
    </el-dialog>
  </el-col>
</el-row>
</template>

<script type="text/ecmascript-6">
import {reqGetRoleList,reqGetOrgList,reqGetAccAdmList,posAccAdm,posModifyAccTch,reqDeleteAccAdm,reqResetAccAdm,reqBlockAccAdm} from '../../api/api'
import PubMethod from '../../common/util'
 export default {
   data() {
     return {
       // 用户令牌
       access_token:'',
       // 表格数据
       AccData: [],
       listLoading:false,

       selectRowIndex:'',
       totalNum:0,
       page:1,
       size:10,

       // 获取单位组织信息用于新增编辑
       OrgData: [],

       // 获取可选角色列表
       RoleData:[],

       // 新增教师表单规则验证
       rules:{
         AccountID:[
           {required: true, message: '请输入账号' , trigger: 'blur'}
         ],
         AccountName:[
           {required: true, message: '请输入姓名' , trigger: 'blur' },
           {pattern: /^[\u4e00-\u9fa5]{1,6}$/,message:'请输入1-6位汉字',trigger:'blur'}
         ],
         Tel:[
           {required: true, message: '请输入电话' , trigger: 'blur' },
          {pattern: /^1\d{10}$/, message: '电话号码格式有误！', trigger: 'blur'}
           
         ]
         
       },

      // 新增表单相关数据
       submitLoading:false,       
       addFormVisible: false,
       addFormBody:{
         AccountName:'',
         AccountID:'',
         RoleID:'',
         OrgID:'',
         Tel:''
       },

       //编辑表单相关数据
       modifyFormVisible:false,
       modifyLoading:false,
       modifyFromBody:{
           AccountName:'',
           AccountID:'',
           OrgID:'',
           Tel:''
       }


     }

   },
   //声明周期调用
   mounted(){
     this.getList();
     this.getOrg();
     this.getRole();
   },

   //方法集合
   methods:{
     //公共类方法--转换冻结状态
     transfState(row){
       return PubMethod.transfState(row)
     },
     // 填充角色信息
     getRole(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetRoleList(param).then((res)=>{
         this.RoleData=res.data.data.list
         this.listLoading=false
         this.RoleData.splice(0,2)
       }).catch((res)=>{
         console.log(res)
         })
     },
     // 填充单位信息
     getOrg(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetOrgList(param).then((res)=>{
         this.OrgData=res.data.data.list
         //console.log(this.OrgData)
         this.listLoading=false
       }).catch((res)=>{
         console.log(res)
         })
     },
     //获取校团委教师列表
     getList(){
       this.listLoading=true
       let param={
           access_token : "11",
           page : this.page,
           size : this.size
           }
           reqGetAccAdmList(param).then((res)=>{
               this.AccData = res.data.data.list
               this.totalNum = res.data.data.count;
               //console.log(this.AccData)
               this.listLoading=false
               }).catch((res)=>{
                   console.log(res)
                   })
            },
     //新增助理信息
     addSubmit(){
       this.$refs['addForm'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           //复制字符串
           let para = Object.assign({}, this.addFormBody);
           para.access_token='terry'
           posAccAdm(para).then((res)=>{
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

          posModifyAccTch(para).then((res)=>{
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
    //重置教师用户密码
    resetAccTch(index,row){
      this.$confirm('此操作将重置该用户密码, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          let para={accountID:row.AccountID}
          para.access_token='terry'
         
          reqResetAccAdm(para).then((res)=>{
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
            this.getList()
          })

            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消重置'
                });          
              });
    },
    // 冻结助理用户
    blockAccAdm(index,row){
       this.$confirm('此操作将冻结该用户, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          let para={accountID:row.AccountID}
          para.access_token='terry'
         
          reqBlockAccAdm(para).then((res)=>{
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
            this.getList()
          })

            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消冻结'
                });          
              });  
    },
    //删除教师用户
    delectAccTch(index,row){
      this.$confirm('此操作将永久删除该用户, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          this.listLoading=true
          let para={accountID:row.AccountID}
          para.access_token='terry'         
          reqDeleteAccAdm(para).then((res)=>{
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
