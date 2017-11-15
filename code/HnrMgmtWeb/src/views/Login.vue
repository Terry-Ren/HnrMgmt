<template >
<div class="login">
  <el-row type="flex" justify="end" class="main-container">
    <el-col :span="12">
      <div class="main-tittle"> 
        <h1>学生荣誉管理信息系统</h1>
        <h1>武汉理工大学团委</h1>
      </div>
    </el-col>
    <el-col :span="9" :offset="3" >
      <el-form :model="loginForm" v-loading="loginLoading" ref="loginForm1" label-position="left" label-width="0px" class=" login-container">
        <h3 class="title">荣誉管理信息系统登录</h3>
        <el-form-item prop="AccountID">
          <el-input type="text" v-model="loginForm.AccountID" auto-complete="off" placeholder="账号"></el-input>
          </el-form-item>
          <el-form-item prop="Password">
            <el-input type="password" v-model="loginForm.Password" auto-complete="off" placeholder="密码"></el-input>
          </el-form-item>
        <el-form-item prop="RoleID">
          <el-select v-model="loginForm.RoleID" placeholder="请选择角色">
            <el-option v-for="Role in RoleIDs" :key="Role.value" :label="Role.label" :value="Role.value"></el-option>
          </el-select>
        </el-form-item>
         <!-- <el-checkbox v-model="checked" checked class="remember">记住密码</el-checkbox> -->
        <el-form-item style="width:100%;">
          <el-button type="primary" style="width:100%;" @click.native.prevent="login">登录</el-button>
          <!--<el-button @click.native.prevent="handleReset2">重置</el-button>-->
         </el-form-item>
      </el-form>
    </el-col>
  </el-row>

  </div>
</template>

<script type="text/ecmascript-6">
import PubMethod from '../common/util'
import * as types from '../store/mutation-types'
import {posLogin} from '../api/api'
 export default {
   data() {
     return {
       loginLoading:false,
         // 表单需要内容
         loginForm:{
             AccountID:'',
             Password:'',
             RoleID:''
         },
         //角色
         RoleIDs:[{
             value:'0',
             label:'超级管理员'
         },{
             value:'1',
             label:'校团委账号'
         },{
             value:'2',
             label:'校团委助理'
         },{
             value:'3',
             label:'学院账号'
         }]

     }
      },
      methods:{
          // 登录方法 
          login(){
            let param
            this.$refs['loginForm1'].validate((valid)=>{
              if(valid){
                this.loginLoading=true
                let para=Object.assign({},this.loginForm)
                para.id=para.AccountID
                para.pwd=para.Password
                posLogin(para).then((res)=>{
                  let resData=res.data    
                  if(resData.status=='success'){
                    this.$message({
                      type: 'success',
                      message: resData.messages
                      })
                      this.$store.commit(types.LOGIN,resData)
                      let redirect = decodeURIComponent(this.$route.query.redirect || '/')
                      this.$router.push({
                        path: redirect
                    })
                    }else{ 
                      this.$message({
                        type: 'error',
                        message: resData.messages
                        })
                        this.loginLoading=false             
                        }})
                  }})}
            }
 }
</script>

<style scoped lang="scss">
.login{
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background:  url(../assets/loginBg.jpg)   ;
  background-position:center;
  background-size:100% 100%;
}
.main-container{
  margin-top: 180px;

  .main-tittle{
    padding-top: 40px;
    font-size: 25px;
    text-align: right;
  }
}
  .login-container {
    // position: absolute;
    // bottom: 250px;
    // right: 200px;
    /*box-shadow: 0 0px 8px 0 rgba(0, 0, 0, 0.06), 0 1px 0px 0 rgba(0, 0, 0, 0.02);*/
    -webkit-border-radius: 5px;
    border-radius: 5px;
    -moz-border-radius: 5px;
    width: 350px;
    padding: 35px 35px 15px 35px;
    background: rgba(255, 255, 255, 0.9);
    border: 1px solid #eaeaea;
    box-shadow: 0 0 25px #cac6c6;
    .title{
        text-align: center;
    }
    }
    .el-select{
            width: 100%;
    }
 
</style>
