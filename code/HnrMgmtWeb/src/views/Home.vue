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
    <aside>
      <!-- 展开关闭按钮 -->
      <el-button  type="primary" @click.prevent="collapse">
        <i class="el-icon-caret-right" v-show="isCollapse"></i>
        <i class="el-icon-caret-left" v-show="!isCollapse"></i>
      </el-button>
      <!-- 主菜单 -->
      <el-menu class="el-menu-vertical-demo" :collapse="isCollapse">
        <el-submenu index="1">
          <template slot="title">
            <i class="el-icon-message"></i>
            <span slot="title">导航一</span>
            </template>
            <el-menu-item-group>
               <el-menu-item index="1-1">选项一</el-menu-item>
               <el-menu-item index="1-1">选项二</el-menu-item>
            </el-menu-item-group>

        </el-submenu>
      </el-menu>
    </aside>
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
    }
    
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

      aside{
        min-width: 50px;
        background: #333744;
        &::-webkit-scrollbar {
          display: none;
          }
      }

      .el-menu {
        height: 100%; /*写给不支持calc()的浏览器*/
        height: -moz-calc(100% - 80px);
        height: -webkit-calc(100% - 80px);
        height: calc(100% - 80px);
        border-radius: 0px;
        background-color: #333744;
      }
            .el-submenu .el-menu-item {
        min-width: 60px;
      }
      .el-menu {
        width: 180px;
      }
      .el-menu--collapse {
        width: 60px;
      }

      .el-menu .el-menu-item, .el-submenu .el-submenu__title {
        height: 46px;
        line-height: 46px;
      }

      .el-menu-item:hover, .el-submenu .el-menu-item:hover, .el-submenu__title:hover {
        background-color: #7ed2df;
      }
          .menu-toggle {
      background: #4A5064;
      text-align: center;
      color: white;
      height: 26px;
      line-height: 30px;
    }




  }
</style>

<style>
  .el-menu-vertical-demo:not(.el-menu--collapse) {
    width: 200px;
    min-height: 400px;
  }
</style>