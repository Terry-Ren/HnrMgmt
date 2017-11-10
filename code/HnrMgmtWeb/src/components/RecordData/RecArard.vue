<template>
<el-row class="warp">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>项目填报</el-breadcrumb-item>
        <el-breadcrumb-item>奖项填报</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
 <!-- 下方主内容 -->
  <el-col :span="24" class="warp-main left-main">
    <!-- 工具栏 -->
    <el-col :span="24" class="toolBar" >    
      <el-form :inline="true" style="margin-bottom:15px">
        <el-button type="primary" @click="addFormVisible = true" >新增奖项</el-button>
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
  </el-col>

    <!-- 新增表单 -->
    <el-dialog title="新增荣誉记录" :visible.sync="addFormVisible" v-loading="submitLoading" style="top:-11%">
      <el-form :model="addFormBody" label-width="110px" ref="addForm" :rules="rules" auto class="hornor-add" >
        <el-form-item label="荣誉项目" prop="AwdID">
          <el-select v-model="addFormBody.AwdID" placeholder="请选择奖项" style="width:300px">
            <el-option v-for="honor in AwardData" :key="honor.AwdID" :value="honor.AwdID" :label="honor.Name+honor.Grade"></el-option>
          </el-select>
        </el-form-item>  
        <el-form-item label="获奖年度" prop="Year" >
          <el-date-picker v-model="addFormBody.Year"     type="year" placeholder="获得年度" style="width:300px" format="yyyy 年 " value-format="yyyy"></el-date-picker>
        </el-form-item>
        <el-form-item label="获奖届数" prop="Term">
          <el-input v-model="addFormBody.Term" placeholder="请输入获奖届数（可空）" style="width:300px" ></el-input>
        </el-form-item>
        <el-form-item label="获奖日期" prop="AwdTime">
          <el-date-picker v-model="addFormBody.AwdTime"  placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
        </el-form-item>
        <el-form-item label="项目名称" prop="ProjectName">
          <el-input v-model="addFormBody.ProjectName" placeholder="请输入获奖项目名" style="width:300px" ></el-input>
        </el-form-item>
        <el-form-item label="项目所属学院" prop="OrgID">
          <el-select v-model="addFormBody.OrgID" placeholder="请选择所属学院" style="width:300px">
            <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
          </el-select>
        </el-form-item> 
        <el-form-item label="是否属于团队" prop="IsTeam">
            <el-radio-group v-model="addFormBody.IsTeam" size="medium">
                <el-radio border label="1">是</el-radio>
                <el-radio border label="0">否</el-radio>
            </el-radio-group>
        </el-form-item>          
        <el-form-item v-for="(teacher,index) in addFormBody.Teacher" :key="teacher.key" label="'指导教师'+index" prop="AwdeeName">
          <el-input v-model="addFormBody.AwdeeName" placeholder="请输入获奖人姓名" style="width:300px" ></el-input>
        </el-form-item> 
        <el-form-item label="所属团支部" prop="Branch">
          <el-input v-model="addFormBody.Branch" placeholder="请输入团支部" style="width:300px" ></el-input>
        </el-form-item>  
        <el-form-item label="上传图片" prop="FileName">
          <el-upload action="http://upload.qiniu.com/"  :data="postData" :on-success="successUpload" :before-upload="beforePicUpload" >
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
import {reqGetAwdList,reqGetOrgList,posRecordHonor,posRecordAward} from '../../api/api'
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
         AwdID:'',
         Year:'',
         Term:'',
         AwdTime:'',
         ProjectName:'',
         IsTeam:'',
         Teacher:[],
         AwdeeName:'',
         AwdeeID:'',
         OrgID:'',
         Branch:'',
         FileUrl:'-1'
       },
      // 表单验证规则
      rules:{
        AwdID:{required:true , message:'请选择荣誉项目', trigger:'blur'},
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
     // 填充荣誉数据
     getHonor(){
       this.listLoading=true
       let param={
         access_token:"11"
       }
       reqGetAwdList(param).then((res)=>{
         this.listLoading=false
         this.AwardData=res.data.data.list
              this.transfGrande()
       }).catch((res)=>{
         console.log(res)
         })       
     },
     // 转换获奖级别
     transfGrande(){
         this.AwardData.forEach((award)=>{
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
     // 获取列表
     getList(){

     },
    //在图片提交前进行验证
    beforePicUpload(file) {  
      const isJPG = file.type === 'image/jpeg'
      const isPNG = file.type === 'image/png'      
      const isLt2M = file.size / 1024 / 1024 < 2
      if (!isJPG&&!isPNG) {
        this.$message.error('上传头像图片只能是 JPG/PNG 格式!')
        return false
      } else if (!isLt2M) {
        this.$message.error('上传证明图片大小不能超过 2MB!')
        return false
      }
      return true
      },
      // 上传成功钩子
      successUpload(res, file, fileLis){
        this.addFormBody.FileUrl=this.$store.state.uploadUrl+res.key
        console.log(this.addFormBody)
      },
     //新增荣誉记录
     addSubmit(){
       this.$refs['addForm'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           //复制字符串
           let para = Object.assign({}, this.addFormBody);
           para.access_token='terry'
           posRecordHonor(para).then((res)=>{
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
