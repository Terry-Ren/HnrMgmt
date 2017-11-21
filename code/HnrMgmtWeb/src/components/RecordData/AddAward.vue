<template>
  <div class="container">
    <!-- 面包屑导航 -->      
    <div class="warp-breadcrum">  
      <el-breadcrumb separator="/">
          <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
          <el-breadcrumb-item>项目填报</el-breadcrumb-item>
          <el-breadcrumb-item :to="{ path: '/record/award' }">奖项填报</el-breadcrumb-item>
          <el-breadcrumb-item>新增奖项</el-breadcrumb-item>
      </el-breadcrumb>        
    </div>
    <!-- 下方主内容 --> 
    <div class="warp-body">
      <!-- 工具栏 -->    
      <div class="toolbal">
        <el-button style="float: right; padding: 3px 0" type="text" @click="backToMain">返回</el-button>              
      </div>
      <!-- 表格区 --> 
      <div class="main-data">
        <div class="modify-box">
          <el-form :model="addFormBody" label-width="110px" ref="addForm" :rules="rules" auto  >
            <el-form-item label="奖项名称" prop="AwardID">
              <el-select v-model="addFormBody.AwardID" placeholder="请选择奖项" style="width:300px">
                <el-option v-for="award in AwardData" :key="award.AwdID" :value="award.AwdID" :label="'【'+award.GradeName+'】'+award.Name+award.Grade"></el-option>
              </el-select>
            </el-form-item>               
            <el-form-item label="获奖年度" prop="AwdYear" >
              <el-date-picker v-model="addFormBody.AwdYear"  type="year" placeholder="获得年度" style="width:300px" format="yyyy 年 " value-format="yyyy"></el-date-picker>
            </el-form-item>
            <el-form-item label="获奖届数" prop="Term">
              <el-input v-model="addFormBody.Term" placeholder="请输入获奖届数（可空）" style="width:300px" ></el-input>
            </el-form-item>
            <el-form-item label="获奖日期" prop="AwdTime">
              <el-date-picker v-model="addFormBody.AwdTime"  placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
            </el-form-item>
            <el-form-item label="项目名称" prop="AwdProName">
              <el-input v-model="addFormBody.AwdProName" placeholder="请输入获奖项目名" style="width:300px" ></el-input>
            </el-form-item>
            <el-form-item label="项目所属学院" prop="AwdOrgID">
              <el-select v-model="addFormBody.AwdOrgID" placeholder="请选择所属学院" style="width:300px">
                <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
              </el-select>
            </el-form-item> 
            <el-form-item label="是否属于团队" prop="IsTeam">
                <el-radio-group v-model="addFormBody.IsTeam" size="medium">
                    <el-radio border label="1">是</el-radio> 
                    <el-radio border label="0">否</el-radio>
                </el-radio-group>
            </el-form-item>
            <el-form-item label="新增人员">
                <el-button type="infor" @click="addMember" round  >新增成员</el-button>
                <el-button type="infor" @click="addTeacher" round  >新增教师</el-button>
            </el-form-item>
            <!-- 新增学生信息 -->
            <el-form-item v-for="(member,index) in addFormBody.Members" :key="member.key" :label="'成员'+'【'+index+'】'">
              <el-input v-model="member.AwdeeID" placeholder="请输入学号" style="width:300px; margin:0 10px 10px 0"></el-input>          
              <el-input v-model="member.AwdeeName" placeholder="请输入姓名" style="width:300px; margin:0 300px 10px 0"></el-input>
              <el-select v-model="member.OrgID" placeholder="请选择学院" style="width:300px; margin:0 10px 10px 0">
                <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
              </el-select>
              <el-input v-model="member.Branch" placeholder="请输入团支部" style="width:300px" >
                <template slot="append">团支部</template>
              </el-input>
              <el-button type="danger" @click.prevent="removeMember(member)">删除成员</el-button>
            </el-form-item> 
            <!-- 新增教师信息-->
            <el-form-item v-for="(teacher,index) in addFormBody.Teacher" :key="teacher.key" :label="'指导教师'+'【'+index+'】'">
              <el-input v-model="teacher.TchName" placeholder="请输入指导教师" style="width:300px;" ></el-input>
              <el-button type="danger" @click.prevent="removeTeacher(teacher)">删除教师</el-button>
            </el-form-item> 
            <el-form-item label="上传图片" prop="FileUrl">
              <el-upload action="http://upload.qiniu.com/"  :data="postData" :on-success="successUpload" :before-upload="beforePicUpload" >
                <el-button size="small" type="primary">点击上传</el-button>
                <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div>
              </el-upload>
            </el-form-item>
            <!-- 提交工具条 -->
            <el-form-item label="">
                <el-button type="primary" @click.native="addSubmit" >提交</el-button>            
            </el-form-item>          
          </el-form>          
        </div>
        <div class="check-box">
           <el-form :model="addFormBody" label-width="100px" ref="addForm" :rules="rules" auto >      
              <el-form-item  label="图片预览">
                <img class="file" src="http://oyzg731sy.bkt.clouddn.com/FlL70dFa87VxKgNSYDJ3AQcfCUr_" alt="暂无证明材料">
              </el-form-item>                                                           
           </el-form>  
        </div>            
      </div>
    </div>
  </div>  
</template>

<script type="text/ecmascript-6">
import { reqGetAwdList, reqGetOrgList, posRecordAward } from "../../api/api";
import PubMethod from "../../common/util";
import * as types from "../../store/mutation-types";
export default {
  data() {
    // 获奖人学号验证规则
    var validateAwdeeID = (rule, value, callback) => {
      const RULES = /^\d{5,13}$/;
      if (value == null) {
        callback(new Error("学号不能为空"));
      } else if (!RULES.test(value)) {
        callback(new Error("必须为5-13位数字"));
      } else {
        callback();
      }
    };
    return {
      // 七牛云令牌
      postData: {
        token: this.$store.state.uploadToken
      },
      // 填充奖项数据
      AwardData: [],
      // 填充组织单位
      OrgData: [],
      // 用户令牌
      access_token: "",
      // 新增表单相关数据
      submitLoading: false,
      addFormVisible: true,
      addFormBody: {
        AwardID: "",
        AwdYear: "",
        Term: "",
        AwdTime: "",
        AwdProName: "",
        IsTeam: "",
        Teacher: [],
        Members: [],
        AwdOrgID: "",
        FileUrl: "-1"
      },
      // 表单验证规则
      rules: {
        AwardID: { required: true, message: "请选择荣誉项目", trigger: "blur" },
        AwdYear: { required: true, message: "请选择获得年度", trigger: "blur" },
        AwdTime: { required: true, message: "请选择获得年月", trigger: "blur" },
        AwdeeID: [
          { required: true, message: "请输入学号", trigger: "blur" },
          { validator: validateAwdeeID, tigger: "blure" }
        ],
        AwdeeName: { required: true, message: "请输入获奖人姓名", trigger: "blur" },
        OrgID: { required: true, message: "请选择单位学院", trigger: "blur" },
        Branch: { required: true, message: "请输入所属团支部", trigger: "blur" }
      }
    };
  },
  //    // 计算属性
  //    computed:{
  //        Branch(){
  //            return this.addFormBody.Branch
  //        }
  //    },
  //    // 观察着
  //    watch:{
  //        Branch(newVal){
  //            this.addFormBody.Branch=newVal+'团支部'
  //        }
  //    },
  //声明周期调用
  mounted() {
    this.getAward();
    this.getOrg();
  },
  methods: {
    // 跳转路由
    backToMain() {
      this.$router.push({
        path: "/record/award"
      });
    },
    // 填充奖项数据
    getAward() {
      this.listLoading = true;
      let param = {
        access_token: "11"
      };
      reqGetAwdList(param)
        .then(res => {
          this.listLoading = false;
          this.AwardData = res.data.data.list;
          this.transfOptionGrande();
          this.transfOptionGrandeName();
        })
        .catch(res => {
          console.log(res);
        });
    },
    // 转换新增选项中的获奖等次
    transfOptionGrande() {
      this.AwardData.forEach(award => {
        award.Grade = PubMethod.transfGrande(award);
      });
    },
    // 转换新增选项中的获奖级别
    transfOptionGrandeName() {
      this.AwardData.forEach(award => {
        award.GradeName = PubMethod.transfGrandeName(award);
      });
    },
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
    // 转换获奖等次
    transfGrande(row) {
      return PubMethod.transfGrande(row);
    },
    //在图片提交前进行验证
    beforePicUpload(file) {
      const isJPG = file.type === "image/jpeg";
      const isPNG = file.type === "image/png";
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isJPG && !isPNG) {
        this.$message.error("上传头像图片只能是 JPG/PNG 格式!");
        return false;
      } else if (!isLt2M) {
        this.$message.error("上传证明图片大小不能超过 2MB!");
        return false;
      }
      return true;
    },
    // 上传成功钩子
    successUpload(res, file, fileLis) {
      this.addFormBody.FileUrl = this.$store.state.uploadUrl + res.key;
    },
    // 新增成员
    addMember() {
      this.addFormBody.Members.push({
        key: Date.now(),
        AwdeeID: "",
        AwdeeName: "",
        OrgID: "",
        Branch: ""
      });
    },
    // 删除成员
    removeMember(member) {
      var index = this.addFormBody.Members.indexOf(member);
      if (index !== -1) {
        this.addFormBody.Members.splice(index, 1);
      }
    },
    // 新增教师
    addTeacher() {
      this.addFormBody.Teacher.push({
        TchName: ""
      });
    },
    // 删除教师
    removeTeacher(teacher) {
      var index = this.addFormBody.Teacher.indexOf(teacher);
      if (index !== -1) {
        this.addFormBody.Teacher.splice(index, 1);
      }
    },
    //新增获奖记录
    addSubmit() {
      //console.log(this.addFormBody)
      this.$refs["addForm"].validate(valid => {
        if (valid) {
          this.submitLoading = true;
          //复制字符串
          let para = Object.assign({}, this.addFormBody);
          para.access_token = "11";
          posRecordAward(para).then(res => {
            this.submitLoading = false;
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this, res.data);
            this.$refs["addForm"].resetFields();
            this.$store.commit(types.RECORD_MODIFY);
            this.backToMain();
          });
        }
      });
    }
  }
};
</script>

<style scoped lang="scss">
.toolbar {
  form {
    display: flex;
    justify-content: space-around;
  }
}
.main-data {
  display: flex;
  position: relative;
  > .modify-box {
    margin: 0 10%;
    //flex:auto;
  }
  > .check-box {
    position: absolute;
    right: 20%;
    //flex:auto;
    .file {
      width: 200px;
      height: 200px;
    }
  }
}
</style>
