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
      <el-form :inline="true" style="margin-bottom:15px">
        <el-button type="primary" @click="addFormVisible = true" >新增荣誉</el-button>
      </el-form>
    </el-col>
    <!-- 表格区 -->
    <el-col :span="24">
      <el-table  :data="HnrData" border style="width:100%" v-loading="listLoading"> 
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column type="index" width="65" label="序号" align="center"></el-table-column>
        <el-table-column prop="Name" label="荣誉名称" sortable align="center"></el-table-column>
        <el-table-column prop="GradeName" label="荣誉级别" align="center" :formatter="transfGrandeName" sortable></el-table-column>
        <el-table-column label="操作" width="150">
          <template slot-scope>
            <el-button  size="small" @click="showModifyDialog(scope.$index,scope.row)" >编辑</el-button>
            <el-button type="danger" size="small"  @click="delectHornor(scope.$index,scope.row)" >删除</el-button>
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
    <el-dialog title="新增荣誉项" :visible.sync="addFormVisible" v-loading="submitLoading" >
      <el-form :model="addFormBody" label-width="80px" ref="addForm" :rules="rules" auto>
        <el-form-item label="荣誉名称" prop="Name">
          <el-input v-model="addFormBody.Name" placeholder="名称不含年份"  ></el-input>
        </el-form-item>
        <el-form-item label="荣誉级别" prop="GradeName">
          <el-select v-model="addFormBody.GradeName" placeholder="请选择级别">
            <el-option v-for="Grade in GradeNames" :key="Grade.value" :label="Grade.label" :value="Grade.value"></el-option>
          </el-select>
        </el-form-item>        
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" addFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="addSubmit" >提交</el-button>
      </div>
    </el-dialog>

    <!-- 编辑表单 -->
    <el-dialog title="编辑荣誉项" :visible.sync="modifyFormVisible" v-loading="modifyLoading">
      <el-form :model="modifyFromBody" label-width="80px" ref="modifyFrom" :rules="rules" >
        <el-form-item label="荣誉名称" prop="Name"  >
          <el-input v-model="modifyFromBody.Name" placeholder="名称不含年份"  ></el-input>
        </el-form-item>
        <el-form-item label="荣誉级别" prop="GradeName">
          <el-select v-model="modifyFromBody.GradeName" placeholder="请选择级别">
            <el-option v-for="Grade in GradeNames" :key="Grade.value" :label="Grade.label" :value="Grade.value"></el-option>
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
import {reqGetHonorList,reqAddHonor,posModifyHonor,reqDeleteHonor} from '../../api/api'
import PubMethod from '../../common/util'
 export default {
   data() {
     return {
       // 用户令牌
       access_token:'',
       // 表格数据
       HnrData: [],
       listLoading:false,

       selectRowIndex:'',
       totalNum:0,
       page:1,
       size:10,

       // 表单规则验证
       rules:{
         Name:{
           required: true, message: '请输入荣誉名称' , trigger: 'blur' 
         },
         GradeName:{
           required: true, message: '请选择级别' , trigger: 'change'
         }

       },

      // 新增表单相关数据
       submitLoading:false,       
       addFormVisible: false,
       addFormBody:{
         Name:'',
         GradeName:''
       },
       GradeNames:[{
         value:'0',
         label:'院级'
       },{
         value:'1',
         label:'校级'
       },{
         value:'2',
         label:'省级'
       },{
         value:'3',
         label:'国级'
       }],

       //编辑表单相关数据
       modifyFormVisible:false,
       modifyLoading:false,
       modifyFromBody:{
         Name:'',
         GradeName:''
       }


     }

   },
   //声明周期调用
   mounted(){
     this.getList();
   },

   //方法集合
   methods:{
     //荣誉级别转换
     transfGrandeName(row){
       return PubMethod.transfGrandeName(row)
       },
     //获取荣誉列表
     getList(){
       this.listLoading=true
       let param={
         page : this.page,
         limit : this.size
       }
       reqGetHonorList(param).then((res)=>{
          this.HnrData = res.data.data.list
          this.totalNum = res.data.data.count;
          //console.log(this.HnrData)
          this.listLoading=false
       }).catch((res)=>{
         console.log(res)
       })
     },
     //新增荣誉项
     addSubmit(){
       let param
       this.$refs['addForm'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           //复制字符串
           let para = Object.assign({}, this.addFormBody);
           reqAddHonor(para).then((res)=>{
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

          posModifyHonor(para).then((res)=>{
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
    //删除功能
    delectHornor(index,row){
      this.$confirm('此操作将永久删除该荣誉项, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          let para={HonorID:row.HonorID}
          para.access_token='terry'
         
          reqDeleteHonor(para).then((res)=>{
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
