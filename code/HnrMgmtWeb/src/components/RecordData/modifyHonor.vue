<template>
<div class="container">
    <!-- 面包屑导航 -->
    <div class="warp-breadcrum">
        <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
            <el-breadcrumb-item>项目填报</el-breadcrumb-item>
            <el-breadcrumb-item :to="{ path: '/record/honor' }">奖项填报</el-breadcrumb-item>
            <el-breadcrumb-item>详情</el-breadcrumb-item>
        </el-breadcrumb>        
    </div>
    <!-- 下方主内容 -->
    <div class="warp-body">
        <!-- 工具条 -->
        <div class="toolbar">
            <el-form :inline="true" style="margin-bottom:15px">
            <el-button type="primary" @click="modifyFormVisible = true" >编辑该项</el-button>
            <el-button type="infor" @click="backToMain" >返回</el-button>            
            </el-form>             
        </div>
        <!-- 主要表单 -->
        <div class="main-data">
            <!-- 新增表单 -->
            <el-form :model="detailFormBody" label-width="100px" ref="addForm" :rules="rules" auto class="hornor-add" style="padding-left: 10%;" >
                <el-form-item label="荣誉项目" prop="HonorID">
                <el-select v-model="detailFormBody.HonorID" placeholder="请选择荣誉" style="width:300px">
                    <el-option v-for="honor in HonorData" :key="honor.HonorID" :value="honor.HonorID" :label="honor.Name"></el-option>
                </el-select>
                </el-form-item>  
                <el-form-item label="获奖年度" prop="Annual" >
                <el-select v-model="detailFormBody.Annual"  placeholder="请选择年度" style="width:300px">
                    <el-option v-for="options in annualOptions" :key="options.value" :label="options.label" :value="options.value"></el-option>
                </el-select>
                </el-form-item>
                <el-form-item label="获奖日期" prop="HnrTime">
                <el-date-picker v-model="detailFormBody.HnrTime"  placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
                </el-form-item>
                <el-form-item label="获奖人学号" prop="AwdeeID">
                <el-input v-model="detailFormBody.AwdeeID" placeholder="请输入获奖人学号" style="width:300px" ></el-input>
                </el-form-item>          
                <el-form-item label="获奖人姓名" prop="AwardeeName">
                <el-input v-model="detailFormBody.AwardeeName" placeholder="请输入获奖人姓名" style="width:300px" ></el-input>
                </el-form-item> 
                <el-form-item label="单位学院" prop="OrgID">
                <el-select v-model="detailFormBody.OrgID" placeholder="请选择所属学院" style="width:300px">
                    <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
                </el-select>
                </el-form-item> 
                <el-form-item label="所属团支部" prop="Branch">
                <el-input v-model="detailFormBody.Branch" placeholder="请输入团支部" style="width:300px" >
                    <template slot="append">团支部</template>
                </el-input>
                </el-form-item>  
                <el-form-item label="">
                    <el-button type="primary" @click.native="addSubmit" >提交</el-button>            
                </el-form-item>                   
            </el-form>            
        </div>
    </div>
</div>
</template>

<script type="text/ecmascript-6">
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
       detailFormBody:{
           AwardeeName:'',          
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
   mounted(){
       this.detailFormBody=this.$store.state.singleHonor
       console.log(this.detailFormBody)
   },
   methods:{
     // 跳转路由
     backToMain(){
       this.$router.push({
         path:'/record/award'
       })
     },       
   }
 }
</script>

<style scoped lang="scss">

 
</style>
