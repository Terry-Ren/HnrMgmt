<template>
  <div class="container">
    <div class="warp-breadcrum">
      <!-- 面包屑导航 -->
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>个人管理</el-breadcrumb-item>
          <el-breadcrumb-item>密码管理</el-breadcrumb-item>
      </el-breadcrumb>    
    </div>
    <div class="warp-body">
      <el-form :model="passwordData"  :rules="rules" ref="passFrom" class="main-from" label-width="100px" v-loading="submitLoading">
        <el-form-item label="原密码" prop="OldPwd">
          <el-input type="password" v-model="passwordData.OldPwd" placeholder="请输入原密码"  ></el-input>  
        </el-form-item>
        <el-form-item label="新密码" prop="NewPwd">
          <el-input type="password" v-model="passwordData.NewPwd" placeholder="请输入新密码"></el-input>
        </el-form-item>
        <el-form-item label="确认密码" prop="checkPass">
          <el-input type="password" v-model="passwordData.checkPass" placeholder="再次输入新密码"></el-input>
        </el-form-item>
        <el-form-item >
          <el-button type="primary" @click="modifyPass">提交</el-button>
          <el-button @click="restFrom(passwordData)">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
import {posModifyPass} from '../../api/api'
import PubMethod from '../../common/util'
 export default {
   data() {
     // 验证规则
     var validatePass = (rule, value, callback) => {
       if (value !== this.passwordData.NewPwd) {
         callback(new Error('两次输入密码不一致!'));
         } else {
           callback();
           }
      };
     return {
       // 表单结构
       passwordData:{
         OldPwd:'',
         NewPwd:'',
         checkPass:''
       },
       // 是否loading
       submitLoading:false,
       // 验证规则
       rules:{
         OldPwd:[
           {required:true, message:'请输入原密码',tigger:'blur'}
         ],
         NewPwd:[
           {required:true, message:'请输入新密码',tigger:'blur'}
         ],
         checkPass:[
           {required:true, message:'请输入新密码',tigger:'blur'},
           {validator:validatePass, tigger:'blure'}
         ]
       }
     }

   },
   methods:{
     // 重置按钮
     restFrom(fromData){
       this.$refs['passFrom'].resetFields()
     },
     // 提交按钮
     modifyPass(){
       this.$refs['passFrom'].validate((valid)=>{
         if(valid){
           this.submitLoading=true
           let param = Object.assign({},this.passwordData)
           param.access_token='terry'
           param.ID=this.$store.state.ID
           posModifyPass(param).then((res)=>{
             PubMethod.statusinfo(this,res.data)
             this.submitLoading=false
             this.$refs['passFrom'].resetFields()
           })
         }
       })
     }
   }
 }
</script>

<style scoped lang="scss">

 
</style>
