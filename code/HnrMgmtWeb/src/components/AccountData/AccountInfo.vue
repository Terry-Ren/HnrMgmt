<template>
   <div class="container">
    <!-- 面包屑导航 -->      
    <div class="warp-breadcrum">   
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>个人管理</el-breadcrumb-item>
          <el-breadcrumb-item>个人信息</el-breadcrumb-item>
      </el-breadcrumb>         
    </div>
    <!-- 下方主内容 --> 
    <div class="warp-body">
      <!-- 工具栏 -->    
      <div class="toolbal"> 
        <el-form :inline="true" style="margin-bottom:15px">
          <el-button v-if="!isModify" type="primary" @click="isModify = true">编辑信息</el-button>
          <el-button v-if="isModify" type="primary" @click="isModify = false">取消编辑</el-button>          
        </el-form>              
      </div>
      <!-- 表格区 --> 
      <div class="main-data"> 
        <div class="control-group">
          <label class="control-label">
            账号
          </label>
          <div class="contorl">
            <span>{{accountData.ID}}</span>
          </div>
          <label class="control-label">
            姓名
          </label>
          <div v-if="!isModify" class="contorl">
            <span>{{accountData.Name}}</span>
          </div> 
          <div v-if="isModify" class="contorl">
            <el-input v-model="accountData.Name" placeholder="请输入姓名" style="width:300px" ></el-input>
          </div>          
          <label class="control-label">
            所在学院
          </label>
          <div class="contorl">
            <span>{{accountData.OrgName}}</span>
          </div> 
          <label class="control-label">
            角色
          </label>
          <div class="contorl">
            <span>{{accountData.RoleName}}</span>
          </div> 
          <label class="control-label">
            手机号
          </label>
          <div v-if="!isModify" class="contorl">
            <span>{{accountData.Tel}}</span>
          </div>
          <div v-if="isModify" class="contorl">
            <el-input v-model="accountData.Tel" placeholder="请输入手机号" style="width:300px" ></el-input>
          </div>
          <div v-if="isModify" class="button">
            <el-button type="" @click="submit" >提交</el-button>
          </div>                                                   
        </div>  
      </div>
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
import PubMethod from "../../common/util";
import { reqGetAccountInfo, posModifyAccountInfo } from "../../api/api";
export default {
  data() {
    return {
      // 是否开启编辑
      isModify: false,
      // 主信息区
      listLoading: false,
      accountData: []
    };
  },
  created() {
    this.getList();
  },
  methods: {
    //获取荣誉列表
    getList() {
      this.listLoading = true;
      let param = {
        access_token: "11"
      };
      reqGetAccountInfo(param)
        .then(res => {
          this.accountData = res.data.data;
          this.listLoading = false;
        })
        .catch(res => {
          PubMethod.logMessage(res);
        });
    },

    // 提交信息修改
    submit() {
      this.listLoading = true;
      let para = Object.assign({}, this.accountData);
      para.access_token = "11";

      posModifyAccountInfo(para).then(res => {
        //公共提示方法，传入当前的vue以及res.data
        PubMethod.statusinfo(this, res.data);
        this.isModify = false;
        this.listLoading = false;
        this.getList();
      });
    }
  }
};
</script>

<style scoped lang="scss">
.main-data {
  display: flex;
  justify-content: space-around;
  align-items: flex-start;
}
.control-group {
  padding-left: 30px;
  padding-top: 30px;
  padding-bottom: 30px;
  width: 350px;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid #eaeaea;
  box-shadow: 0 0 25px #cac6c6;
  display: flex;
  flex-direction: column;
  .control-label {
    font-weight: bold;
    font-size: 1.2em;
    line-height: 2;
    font-family: Helvetica, Arial, sans-serif;
    margin-bottom: 5px;
  }
  .contorl {
    color: #222;
    font-family: Helvetica, Arial, sans-serif;
    font-size: 14px;
    line-height: 19px;
    direction: ltr;
    padding-bottom: 15px;
  }
  .button {
    font-weight: bold;
    padding-left: 15px;
    text-align: center;
  }
}
</style>
