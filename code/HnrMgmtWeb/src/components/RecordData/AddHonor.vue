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
    <el-card class="addFrom" v-loading="submitLoading">
        <div slot="header" class="clearfix">
            <span>新增荣誉项</span>
            <el-button style="float: right; padding: 3px 0" type="text" @click="backToMain">返回</el-button>
            </div>
 
    <!-- 新增表单 -->
      <el-form :model="addFormBody" label-width="100px" ref="addForm" :rules="rules" auto class="hornor-add" style="padding-left: 10%;" >
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
        <el-form-item label="获奖日期" prop="HnrTime">
          <el-date-picker v-model="addFormBody.HnrTime"  placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
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
          <el-input v-model="addFormBody.Branch" placeholder="请输入团支部" style="width:300px" >
            <template slot="append">团支部</template>
          </el-input>
        </el-form-item>  
        <el-form-item label="上传图片" prop="FileUrl">
          <el-upload action="http://upload.qiniu.com/"  :data="postData" :on-success="successUpload" :before-upload="beforePicUpload" >
            <el-button size="small" type="primary">点击上传</el-button>
            <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div>
          </el-upload>
        </el-form-item>
        <el-form-item label="">
            <el-button type="primary" @click.native="addSubmit" >提交</el-button>            
        </el-form-item>                   
      </el-form>
    </el-card>      
  </el-col>
</el-row>
</template>

<script type="text/ecmascript-6">
import {reqGetHonorList,reqGetOrgList,posRecordHonor} from '../../api/api'
import PubMethod from '../../common/util'
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
     // 新增表单相关数据
       submitLoading:false,       
       addFormBody:{
         HonorID:'',
         Annual:'',
         HnrTime:'',
         AwdeeName:'',
         AwdeeID:'',
         OrgID:'',
         Branch:'',
         FileUrl:'-1'
       },
      // 表单验证规则
      rules:{
        HonorID:{required:true , message:'请选择荣誉项目', trigger:'blur'},
        Annual:{required:true , message:'请选择获得年度', trigger:'blur'},
        HnrTime:{required:true , message:'请选择获得年月', trigger:'blur'},
        AwdeeID:[
          {required:true , message:'请输入学号', trigger:'blur'},
          {validator:validateAwdeeID, tigger:'blure'},          
        ],
        AwdeeName:{required:true , message:'请输入获奖人姓名', trigger:'blur'},
        OrgID:{required:true , message:'请选择单位学院', trigger:'blur'},
        Branch:{required:true , message:'请输入所属团支部', trigger:'blur'}         
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
     this.getHonor();
     this.getOrg();
   },
   methods:{
     // 跳转路由
     backToMain(){
       this.$router.push({
         path:'/record/honor'
       })
     },
    // 填充荣誉数据
     getHonor(){
       this.submitLoading=true
       let param={
         access_token:"11"
       }
       reqGetHonorList(param).then((res)=>{
         this.submitLoading=false
         this.HonorData=res.data.data.list
       }).catch((res)=>{
         console.log(res)
         })       
     },
    // 填充单位数据
     getOrg(){
       this.submitLoading=true
       let param={
         access_token:"11"
       }
       reqGetOrgList(param).then((res)=>{
         this.submitLoading=false
         this.OrgData=res.data.data.list
       }).catch((res)=>{
         console.log(res)
         })       
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
      },
    //新增荣誉记录
     addSubmit(){
       this.$refs['addForm'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           //复制字符串
           let para = Object.assign({}, this.addFormBody);
           para.Branch=para.Branch+'团支部'
           para.access_token='terry'
           posRecordHonor(para).then((res)=>{
              this.submitLoading=false
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this,res.data)
              this.$refs['addForm'].resetFields();
              this.backToMain()
           })           
         }
       })
     },
   }
 }
</script>

<style scoped lang="scss">
.left-main{
  border-radius: 5px;
  border: 2px;
}


</style>
