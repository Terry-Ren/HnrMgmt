<template>
<!-- 容器 -->
<div class="home-container">
    <!-- 头部 -->
    <header class="top-bar">
        <!-- 头部logo -->
        <div class="logo">
            <a href="/"></a>            
        </div>
        <!-- 头部标题 -->
        <div class="title">
            <span>学生荣誉管理系统</span>
        </div>
        <!-- 头部账户 -->
        <div class="account">
            <el-dropdown trigger="click" > 
                <span class="dropdown-main">
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
                        <router-link to="/account/info"><span style=" color: #000; font-size: 14px;">个人信息</span></router-link>
                    </el-dropdown-item>
                    <el-dropdown-item>
                        <router-link to="/account/PassWord"><span style=" color: #000; font-size: 14px;">修改密码</span></router-link>
                    </el-dropdown-item>
                    <el-dropdown-item divided>
                        <span style="font-size: 14px;" @click.prevent="logout">退出登录</span>
                        </el-dropdown-item>
                </el-dropdown-menu>
            </el-dropdown>            
        </div>
    </header>
    <!-- 下方主内容 -->
    <section class="body-container">
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
                <!-- 统计查询 -->
                <el-submenu index="ExploreData">
                        <template slot="title">
                            <i class="el-icon-search"></i>
                            <span slot="title">统计查询</span>
                        </template>
                        <el-menu-item index="/Explore/query" >
                            数据统计
                        </el-menu-item>
                        <el-menu-item index="/account/PassWord">
                            数据查询
                        </el-menu-item>
                </el-submenu>                 
                <!-- 项目管理 -->
                <el-submenu index="recordData">
                    <template slot="title">
                        <i class="el-icon-plus"></i> 
                        <span>记录管理</span>
                    </template>
                        <el-menu-item index="/record/honor">
                            荣誉填报
                        </el-menu-item>
                        <el-menu-item index="/record/award">
                            奖项填报
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
                <!-- 个人信息 -->
                <el-submenu index="accountData">
                        <template slot="title">
                            <i class="el-icon-star-off"></i>
                            <span slot="title">个人管理</span>
                        </template>
                        <el-menu-item index="/account/info" >
                            个人信息
                        </el-menu-item>
                        <el-menu-item index="/account/PassWord">
                            修改密码
                        </el-menu-item>
                </el-submenu>                
            </el-menu>
        </aside>
        <section class="main-container">
            <!-- 需要长时间存活的 -->          
            <transition >
            <keep-alive>
                <router-view v-if="$route.meta.keepAlive"></router-view>
            </keep-alive>
            </transition> 
            <!-- 不需要长时间保存的 -->
            <transition mode="out-in">
            <router-view v-if="!$route.meta.keepAlive"></router-view>          
            </transition>
        </section>
    </section>
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
  $background-color: #373d41;
  .home-container{
      display: flex;
      width: 100vw;
      height: 100vh;
      flex-direction: column;
      .top-bar{
          $heigt:50px;
          height: $heigt;
          line-height: 50px;
          background-color: $background-color;
          flex-grow: 0;
          color: #fff;
          >.logo{
              float: left;
              width: 59px;
              height: $heigt;
              display: flex;
              background-image: url('../assets/logo.png');
              background-size: 100% 100%;
              >a{
                  flex-grow: 1;
              }
          }
          >.title{
              float: left;
              padding-left: 10px;
              border-left: 1px solid #000;
              >span{
                  font-size: 20px;
              }
          }
          >.account{
              float: right;
              padding-right: 15px;
              .dropdown-main{
                  color: #fff;
                  cursor: pointer;
                  padding-left: 12px;
              }
          }        
      }
    //   下方主内容
    >.body-container{
        overflow: hidden; // 必须使用
        flex-grow: 1;
        display: flex;
        >aside{
            flex:none;
            background-color: #333744;
            &::-webkit-scrollbar {
            display: none;
            }
            &.showSidebar{
                overflow-x: hidden;
                overflow-y: auto;
            }
           >.asid-button{
               background: #4A5064;
               text-align: center;
               color: white;
               height: 30px;
               line-height: 30px;
               cursor: pointer;
               }
           >.el-menu{
                height: 100%; /*写给不支持calc()的浏览器*/
                height: -moz-calc(100% - 80px);
                height: -webkit-calc(100% - 80px);
                height: calc(100% - 80px);
                border-radius: 0px;
                background-color: #333744;
                width: 180px;           
           }
           >.el-menu--collapse {
               width: 60px;
               }          
        }
        >.main-container{
            flex-grow: 1;
            padding: 10px;
            position: relative;
        }
    }
  }

 
</style>
