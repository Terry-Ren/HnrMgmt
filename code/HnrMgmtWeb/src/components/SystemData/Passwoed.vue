<template>
<el-row class="change-password">
  <el-col :span="24" class="warp-breadcrum">
    <!-- 面包屑导航 -->
    <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
        <el-breadcrumb-item>系统管理</el-breadcrumb-item>
        <el-breadcrumb-item>密码管理</el-breadcrumb-item>
      </el-breadcrumb>
  </el-col>
  <el-col :span="24" class="main-body">
      <el-form :model="passwordData"  :rules="rules" ref="passFrom" class="main-from" label-width="100px">
        <el-form-item label="原密码" prop="oldPass">
          <el-input v-model="passwordData.oldPass" placeholder="请输入原密码"  ></el-input>  
        </el-form-item>
        <el-form-item label="新密码" prop="newPass">
          <el-input v-model="passwordData.newPass" placeholder="请输入新密码"></el-input>
        </el-form-item>
        <el-form-item label="确认密码" prop="checkPass">
          <el-input  v-model="passwordData.checkPass" placeholder="再次输入新密码"></el-input>
        </el-form-item>
        <el-form-item >
          <el-button type="primary">提交</el-button>
          <el-button @click="restFrom(passwordData)">重置</el-button>
        </el-form-item>
      </el-form>
  </el-col>
</el-row>

</template>

<script type="text/ecmascript-6">
 export default {
   data() {
     return {
       passwordData:{
         oldPass:'',
         newPass:'',
         checkPass:''
       },
       rules:{
         oldPass:[
           {required:true, message:'请输入原密码',tigger:'blur'}
         ],
         newPass:[
           {required:true, message:'请输入新密码',tigger:'blur'}
         ],
         checkPass:[
           {required:true, message:'请确认新密码',tigger:'blur'}
         ]
       }
     }

   },
   methods:{
     restFrom(fromData){
       this.$refs['passFrom'].resetFields()
     }
   }
 }
</script>

<style scoped lang="scss">

 
</style>
