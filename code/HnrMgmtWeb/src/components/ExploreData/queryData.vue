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
          <el-form-item label="年度" prop="orgID">
            <el-select v-model="listenData.Time" placeholder="请选择年度" style="width:150px">
              <el-option v-for="annual in annual" :key="annual.value" :value="annual.value" :label="annual.label"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="学院" prop="orgID">
            <el-select v-model="listenData.AwardeeOrgID" placeholder="请选择学院(可多选)" style="width:300px" multiple>
              <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="学生姓名">
            <el-input v-model="listenData.AwardeeName" placeholder="请输入姓名" style="width:150px"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="filterparam">查询</el-button>
          </el-form-item>
        </el-form>
      </div>
      <!-- 表格区 -->
      <div class="main-data">
        <el-table class="table" :data="awdData" style="width:100%" v-loading="listLoading" height="string" :default-sort="{prop: 'GradeName', order: 'descending'}">
          <el-table-column type="selection" width="55"></el-table-column>
        </el-table>
        <el-table class="table" :data="hnrData" style="width:100%" v-loading="listLoading" height="string" :default-sort="{prop: 'GradeName', order: 'descending'}">
          <el-table-column type="selection" width="55"></el-table-column>
        </el-table>
      </div>
    </div>
  </div>
</template>

<script type="text/ecmascript-6">
  import {
    reqGetOrgList,
    queryDataInfo
  } from "../../api/api";
  import {
    annual
  } from '../../assets/data/basic'
  export default {
    data() {
      return {
        //监听对象
        listenData: {
          Time: '',
          AwardeeOrgID: '',
          AwardeeName: '',
          OrgID: ''
        },
        // 查询选项
        queryOption: {
          type: 0,
          conditions: []
        },
        //查询填充项
        OrgData: [],
        annual,
        // 显示区
        listLoading: false,
        //查询结果填充
        awdData: [],
        hnrData: []
      };
    },
    computed: {
      orgfilling() {
        return this.listenData.AwardeeOrgID
      }
    },
    watch: {
      // 监听选项的变动
      orgfilling(newval) {
        this.listenData.OrgID = newval
      }
    },
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
          console.log(this.listenData[prop]);
          if (this.listenData[prop] != "") {
            this.queryOption.conditions[paramNum] = {
              fieldName: prop,
              fieldValues: []
            };
            console.log(this.queryOption.conditions[paramNum].fieldName);
            console.log(paramNum)
            //将数组或者变量压入
            if (typeof this.listenData[prop] === "object") {
              for (var value of this.listenData[prop]) {
                this.queryOption.conditions[paramNum].fieldValues.push({
                  item: value
                });
              }
            } else
              this.queryOption.conditions[paramNum].fieldValues.push({
                item: this.listenData[prop]
              });
            // console.log(this.queryOption.conditions);
            paramNum++;
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
          this.awdData = res.data.data.awdList
          this.hnrData = res.data.data.hnrList
          this.listLoading = false;
        });
      },
      //更换每页数量
      SizeChangeEvent(val) {
        this.size = val;
        this.getList();
        PubMethod.logMessage(this.page + "   " + this.size);
      },
      //页码切换时
      CurrentChangeEvent(val) {
        this.page = val;
        this.getList();
        PubMethod.logMessage(this.page + "   " + this.size);
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

  .main-data {
    width: 100%;
    flex: 1;
    overflow: auto;
    display: flex;
    flex-flow: row nowrap;
  }

</style>
