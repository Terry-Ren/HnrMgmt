<template>
  <div class="container">
    <!-- 面包屑导航 -->
    <div class="warp-breadcrum">
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }">
          <b>首页</b>
        </el-breadcrumb-item>
        <el-breadcrumb-item>统计查询</el-breadcrumb-item>
        <el-breadcrumb-item>数据查询</el-breadcrumb-item>
      </el-breadcrumb>
    </div>
    <!-- 下方主内容 -->
    <div class="warp-body">
      <!-- 工具栏 -->
      <div class="toolbal">
        <el-form :model="queryOption" :inline="true" class="query-option">
          <el-form-item label="学院" prop="orgID">
            <el-select v-model="listenData.orgID" placeholder="请选择学院" style="width:150px">
              <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="学生姓名">
            <el-input v-model="listenData.name" placeholder="请输入姓名" style="width:150px"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="filterparam">查询</el-button>
          </el-form-item>
        </el-form>
      </div>
      <!-- 表格区 -->
      <div class="main-data">
        <el-table class="table" v-loading="listLoading" height="string" >
        </el-table>
      </div>
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
import { reqGetOrgList, queryDataInfo } from "../../api/api";
export default {
  data() {
    return {
      // 显示区
      listLoading: false,
      //监听对象
      listenData: {
        AwardeeOrgName: "",
        name: ""
      },
      // 查询选项
      queryOption: {
        type: "0",
        conditions: []
      },
      //查询填充项
      OrgData: []
    };
  },
  // watch: {
  //   // 监听选项的变动
  //   listenData:{
  //     handler:function(newVal){
  //       console.log(newVal)
  //       for(var i in newVal){
  //         console.log(i)
  //       }
  //       this.queryOption.conditions.push({
  //         fieldName:newVal
  //       })
  //       console.log(this.queryOption)
  //     },
  //     deep:true
  //   }
  // },
  mounted() {
    this.getOrg();
  },
  methods: {
    // 填充单位数据
    getOrg() {
      this.listLoading = true;
      let param = {
        access_token: "11"
      };
      reqGetOrgList(param)
        .then(res => {
          this.listLoading = false;
          this.OrgData = res.data.data.list;
        })
        .catch(res => {
          console.log(res);
        });
    },
    // 过滤所选参数，填充进conditions里面
    filterparam() {
      let paramNum = 0;
      for (var prop in this.listenData) {
        if (this.listenData[prop] != "") {
          this.queryOption.conditions.push({
            fieldName: prop,
            fieldValues: this.listenData[prop]
          });
        }
      }
      this.queryData();
    },
    // 根据参数查询
    queryData() {
      this.listLoading = true;
      let param = Object.assign({}, this.queryOption);
      param.access_token = 11;
      queryDataInfo(param).then(res => {
        console.log(res.data);
      });
    }
  }
};
</script>

<style scoped lang="scss">
.toolbal {
  padding: 20px;
  border: 2px solid #000;
  .el-form-item {
    margin-bottom: 0px;
  }
  .query-option {
    display: flex;
    align-items: center;
  }
}
</style>
