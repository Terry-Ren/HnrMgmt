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
        <el-table-column type="index" width="60"></el-table-column>
        <el-table-column prop="Name" label="荣誉名称" sortable></el-table-column>
        <el-table-column prop="GradeName" label="荣誉级别" :formatter="transfGrande" sortable></el-table-column>
        <el-table-column label="操作" width="150">
          <template scope="scope">
            <el-button  size="small" @click="showModifyDialog(scope.$index,scope.row)" >编辑</el-button>
            <el-button type="danger" size="small"  @click="delectHornor(scope.$index,scope.row)" >删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-col>
    <!-- 下方工具条 -->
    <el-col :span="24">
      <el-pagination   small layout="prev, pager, next" :total="20">
  </el-pagination>
    </el-col>

    <!-- 新增表单 -->
    <el-dialog title="新增荣誉项" :visible.sync="addFormVisible" v-loading="submitLoading">
      <el-form :model="addFormBody" label-width="80px" ref="addForm" auto>
        <el-form-item label="荣誉名称" >
          <el-input v-model="addFormBody.Name" placeholder="名称不含年份"  ></el-input>
        </el-form-item>
        <el-form-item label="荣誉级别">
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
    <el-dialog title="编辑荣誉项" :visible.sync="modifyFormVisible">
      <el-form :model="modifyFromBody" label-width="80px" ref="modifyFrom" >
        <el-form-item label="荣誉名称" >
          <el-input v-model="modifyFromBody.Name" placeholder="名称不含年份"  ></el-input>
        </el-form-item>
        <el-form-item label="荣誉级别" >
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
 export default {
   data() {
     return {
       //用户令牌
       access_token:'',
       //表格数据
       HnrData: [],
       listLoading:false,
       submitLoading:false,
       selectRowIndex:'',

      //新增表单相关数据
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
     transfGrande(row){
       return row.GradeName == '0' ? '院级' : row.GradeName == '1' ? '校级' : row.GradeName == '2' ? '省级': '国级';
     },
     //获取荣誉列表
     getList(){
       this.listLoading=true
       let param
       reqGetHonorList(param).then((res)=>{
          this.HnrData = res.data.data.list
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
              this.$message({
                  message: '提交成功',
                  type: 'success'
               });
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
          this.modifyFormVisible=true;
          let para = Object.assign({},this.modifyFromBody)
          para.access_token='terry'

          posModifyProfile(para).then((res)=>{
            this.$message({
              message: '提交成功',
              type: 'success'
              });
              this.$refs['modifyFrom'].resetFields()
              this.modifyFormVisible=false
              this.getList()
          })
        }
      })
    },
    //删除功能
    delectHornor(index,row){
      this.$confirm('此操作将永久删除该文件, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
        }).then(() => {
          let para={HonorID:row.HonorID}
          para.access_token='terry'
         
          reqDeleteHonor(para).then((res)=>{
            this.$message({
              type: 'success',
              message: '删除成功!'
            })
            this.getList()
          })

            }).catch(() => {
              this.$message({
                type: 'info',
                message: '已取消删除'
                });          
              });
    }
  }
 }
</script>

<style scoped lang="scss">

 
</style>
