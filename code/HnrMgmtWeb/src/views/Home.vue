<template>
<!-- 容器 -->
<el-row class="all-containers">
  <!-- 头部 -->
  <el-col :span="24" class="top-bar-warp">
    <div class="top-bar-logo">
      <a href="/"><img src="../assets/logo.png" alt="logo"></a>
    </div>
    <div class="top-bar-title">
      <span style="font-size: 18px;color: #fff;">学生荣誉管理系统</span>
    </div>
  </el-col>

  <!-- 中间部分,felx -->
  <el-col :span="24" class="body-containers">
    <!-- 左侧导航区 -->
    <aside :class="{showSidebar:!isCollapse}">
      <!-- 展开关闭按钮 -->
      <el-button  type="primary" @click.prevent="collapse">
        <i class="el-icon-caret-right" v-show="isCollapse"></i>
        <i class="el-icon-caret-left" v-show="!isCollapse"></i>
      </el-button>
      <!-- 主菜单 -->
      <el-menu class="el-menu-vertical-demo" :collapse="isCollapse">
        <!-- 首页 -->
        <el-menu-item index="index">
          <i class="el-icon-menu "></i>
          <span slot="title">首页</span>
        </el-menu-item>
        <!-- 系统管理 -->
        <el-submenu index="SysManage">
          <template slot="title">
            <i class="el-icon-edit "></i>
            <span slot="title">系统管理</span>
          </template>
          <el-menu-item index="User">
            账户管理
          </el-menu-item>
          <el-menu-item index="Role">
            角色管理
          </el-menu-item>
          <el-menu-item index="PassWord">
            密码管理
          </el-menu-item>
        </el-submenu>
        <!-- 基础数据 -->
        <el-submenu index="JcData">
          <template slot="title">
            <i class="el-icon-setting"></i>
            <span slot="title">基础数据</span>
          </template>
          <el-menu-item index="HonorManag">
            荣誉项目管理
          </el-menu-item>
          <el-menu-item index="AwardManag">
            竞选项目管理
          </el-menu-item>
          <el-menu-item index="OrganiManag">
            单位学院管理
          </el-menu-item>
        </el-submenu>
      </el-menu>
    </aside>

    <!-- 右侧内容区 -->
    <section class="main-containers">
      <div>
        <el-col :span="24" class="main-warp">
        <transition name="fade" model="out-in" >
          <router-view></router-view>
        </transition>
        </el-col>
      </div>     
    </section>
  </el-col>

</el-row>
 </div>
</template>

<script type="text/ecmascript-6">
 export default {
   data() {
	 return {
      isCollapse: true
	 }

   },
   components: {

   },
   //折叠
   methods:{
     collapse: function () {
       this.isCollapse = !this.isCollapse;
       },
   }
 }
</script>

<style scoped lang="scss">
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
        
        &.showSidebar {
          overflow-x: hidden;
          overflow-y: auto;}

      }

      //左侧内容样式
      aside{
        min-width: 50px;
        background: #333744;
        &::-webkit-scrollbar {
          display: none;
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
        }

      





  }
</style>

<style>
  .el-menu-vertical-demo:not(.el-menu--collapse) {
    width: 200px;
    min-height: 400px;
  }
</style>