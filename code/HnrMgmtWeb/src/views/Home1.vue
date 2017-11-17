<template>
<!-- 容器 -->
<el-row class="all-containers">
  <!-- 头部 -->
  <el-col :span="24" class="top-bar-warp">
    <!-- logo -->
    <div class="top-bar-logo">
      <a href="/"><img src="../assets/logo.png" alt="logo"></a>
    </div>
    <!-- 标题 -->
    <div class="top-bar-title">
      <span >学生荣誉管理系统</span>
    </div>
    <!-- 账户 -->
    <div class="top-bar-account">
      <el-dropdown trigger="click" > 
        <span class="el-dropdown-main">
          <svg class="icon" aria-hidden="true">
            <use xlink:href="#honor-account"></use>
            </svg>
            <span>{{RoleName}}</span>
            <svg class="icon" aria-hidden="true">
            <use xlink:href="#honor-moreunfold"></use>
            </svg>
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item>
             <router-link to="/hnrlist"><span style=" color: #000; font-size: 14px;">个人信息</span></router-link>
          </el-dropdown-item>
          <el-dropdown-item>
             <router-link to="/system/PassWord"><span style=" color: #000; font-size: 14px;">修改密码</span></router-link>
          </el-dropdown-item>
          <el-dropdown-item divided>
             <span style="font-size: 14px;" @click.prevent="logout">退出登录</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
  </el-col>

  <!-- 中间部分,felx -->
  <el-col :span="24" class="body-containers">
    <!-- 左侧导航区 -->
    <aside :class="{showSidebar:!isCollapse}">
      <!-- 展开关闭按钮 -->
      <div class="asid-button" @click.prevent="collapse">
          <svg class="icon" aria-hidden="true" v-show="isCollapse">
            <use xlink:href="#honor-more"></use>
          </svg>
          <svg class="icon" aria-hidden="true" v-show="!isCollapse">
            <use xlink:href="#honor-back"></use>
          </svg>
      </div>
      <!-- 主菜单 -->
      <el-menu  :collapse="isCollapse" :default-active="$route.path" router>
        <!-- 首页 -->
        <el-menu-item index="/" > 
          <i class="el-icon-menu "></i>
          <span slot="title">首页</span>
        </el-menu-item>
        <!-- 项目管理 -->
        <el-submenu index="recordData">
          <template slot="title">
            <i class="el-icon-plus"></i> 
            <span>项目填报</span>
          </template>
          <el-menu-item index="/record/honor">
            荣誉填报
          </el-menu-item>
          <el-menu-item index="/record/award">
            奖项填报
          </el-menu-item>
          <el-menu-item index="">
            不知道是啥
          </el-menu-item>
          <el-menu-item index="">
            这个也不知道是啥
          </el-menu-item>
        </el-submenu>
        <!-- 系统管理 -->
        <el-submenu index="SysManage">
          <template slot="title">
            <i class="el-icon-edit "></i>
            <span slot="title">系统管理</span>
          </template>
          <el-menu-item index="/system/acclist">
            账户管理
          </el-menu-item>
          <el-menu-item index="/system/Role">
            角色管理
          </el-menu-item>
          <el-menu-item index="/system/PassWord">
            密码管理
          </el-menu-item>
          <el-menu-item index="/system/Menulist">
            权限管理
          </el-menu-item>
        </el-submenu>
        <!-- 基础数据 -->
        <el-submenu index="basicData">
          <template slot="title">
            <i class="el-icon-setting"></i>
            <span slot="title">基础数据</span>
          </template>
          <el-menu-item index="/basic/hnrlist" >
            荣誉项目管理
          </el-menu-item>
          <el-menu-item index="/basic/awdlist">
            竞赛项目管理
          </el-menu-item>
          <el-menu-item index="/basic/orglist">
            单位学院管理
          </el-menu-item>
        </el-submenu>
      </el-menu>
    </aside>

    <!-- 右侧内容区 -->
    <section class="main-containers">
      <div>
        <el-col :span="24" class="main-warp">
          <!-- 需要长时间存活的 -->          
        <transition name="fade" model="out-in" >
          <keep-alive>
            <router-view v-if="$route.meta.keepAlive"></router-view>
          </keep-alive>
        </transition  name="fade" model="out-in"> 
          <!-- 不需要长时间保存的 -->
        <transition>
          <router-view v-if="!$route.meta.keepAlive"></router-view>          
        </transition>
        </el-col>
      </div>     
    </section>
      <!-- 底部栏 -->
      <!-- <el-col :span="24" class="bottom-containers">
        <span>Copyright © 1999-2017, RWS, All Rights Reserved </span>
        </el-col> -->
  </el-col>


</el-row>
 </div>
</template>

<script type="text/ecmascript-6">
import * as types from '../store/mutation-types'
 export default {
   data() {
	 return {
      isCollapse: false,
      RoleName:this.$store.state.Name
	 }

   },
   components: {

   },
   methods:{
      //折叠     
      collapse: function () {
       this.isCollapse = !this.isCollapse;
       },
       // 登出
       logout(){
         this.$store.commit(types.LOGOUT)
         this.$router.push({
           path:'/login'
         })
       }
       
   }
 }
</script>

<style scoped lang="scss">
//scoped范围样式，用于隔离组件的style
  .all-containers{
    position: absolute;
    top: 0px;
    bottom: 0px;
    width: 100%;
    left: 0px;
    
    
    .top-bar-warp{
      height: 50px;
      line-height: 50px;
      background: #373d41;
      padding: 0px;
      color: #fff;
      
      .top-bar-logo{
        float: left;
        width: 59px;
        line-height: 26px;
        }
      
      .top-bar-logo img{
        height: 40px;
        margin-top: 5px;
        margin-left: 2px;
        }
      
      .top-bar-title {
        float: left;
        text-align: left;
        width: 200px;
        padding-left: 10px;
        border-left: 1px solid #000;
        span{
         font-size: 20px;
        }
        }

        .top-bar-account{
          float: right;
          padding-right: 15px;
          .el-dropdown-main{
          color: #fff;
          cursor: pointer;
          padding-left: 12px;
          }
        }
        
      } //top-bar-warp包裹

      .body-containers{
        display: -webkit-box;
        display: -webkit-flex;
        display: -ms-flexbox;
        display: flex;
        position: absolute;
        top: 50px;
        bottom: 0px;
        overflow: hidden;
      }

      //左侧内容样式
      aside{
        min-width: 50px;
        background: #333744;
        &::-webkit-scrollbar {
          display: none;
        }

        .asid-button{
          background: #4A5064;
          text-align: center;
          color: white;
          height: 30px;
          line-height: 30px;
          cursor: pointer;
        }
        
        &.showSidebar {
          overflow-x: hidden;
          overflow-y: auto;
          }


        .el-menu {
          height: 100%; /*写给不支持calc()的浏览器*/
          height: -moz-calc(100% - 80px);
          height: -webkit-calc(100% - 80px);
          height: calc(100% - 80px);
          border-radius: 0px;
          background-color: #333744;
      }
      .el-menu {
        width: 180px;
      }
      .el-menu--collapse {
        width: 60px;
      }

      }
      //中间主内容样式
      .main-containers{
        background: #fff;
        flex: 1;
        overflow-y: auto;
        padding: 10px;
        padding-bottom: 1px;
        
        .main-warp{
           background-color: #000000 red;
           box-sizing: border-box;
        }
              //底部样式
      // .bottom-containers{
      //   position: absolute;
      //   height: 50px;
      //   background: #333744;
      //   bottom:0px;
      //   border: 1px solid #fff;
      //   padding: 12px;

      //   SPAN{
      //     text-align: center;
      //     color: white;
      //     font-size: 14px;
      //   }
      // }
        }
  }
</style>
