<template>
<div class="container">
    <!-- 面包屑导航 -->
    <div class="warp-breadcrum">
        <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
            <el-breadcrumb-item>记录管理</el-breadcrumb-item>
            <el-breadcrumb-item :to="{ path: '/record/award' }">奖项管理</el-breadcrumb-item>
            <el-breadcrumb-item>详情</el-breadcrumb-item>
        </el-breadcrumb>        
    </div>
    <!-- 下方主内容 -->
    <div class="warp-body">
        <!-- 工具条 -->
        <div class="toolbar">
            <el-form :inline="true" style="margin-bottom:15px">
            <el-button v-if="!ismodify" type="primary" @click="selectModify" >编辑该项</el-button>
            <el-button v-if="ismodify" type="primary" @click="ismodify=false" >取消编辑</el-button>
            <el-button v-if="!ismodify" @click="showTeam" >查看团队</el-button>                        
            <el-button  @click="backToMain" >返回</el-button>            
            </el-form>             
        </div>
        <!-- 主要表单 -->
        <div class="main-data" v-loading="submitLoading">
          <div class="modify-box">
            <!-- 编辑表单 -->
            <el-form :model="detailFormBody" label-width="110px" ref="modifyFrom" :rules="rules" auto>
                <el-form-item v-if="!ismodify" label="奖项名称" prop="AwdName" >
                    <el-input v-model="detailFormBody.AwdName" :disabled="!ismodify"  style="width:300px" ></el-input>                                   
                </el-form-item>                
                <el-form-item v-if="ismodify" label="奖项名称" prop="AwardID">
                    <el-select  v-model="detailFormBody.AwardID" :placeholder="detailFormBody.AwdName" style="width:300px">
                        <el-option v-for="award in AwardData" :key="award.AwdID" :value="award.AwdID"  :label="'【'+award.GradeName+'】'+award.Name+award.Grade"></el-option>
                    </el-select>
                </el-form-item> 
                <el-form-item v-if="!ismodify" label="奖项级别" prop="Grade" >
                    <el-input v-model="detailFormBody.Grade" :disabled="!ismodify"   style="width:300px" ></el-input>                                   
                </el-form-item>                  
                <el-form-item label="获奖年度" prop="AwdYear" >
                  <el-date-picker v-model="detailFormBody.AwdYear" :disabled="!ismodify"  type="year" placeholder="获得年度" style="width:300px" format="yyyy 年 " value-format="yyyy"></el-date-picker>
                </el-form-item>
                <el-form-item label="获奖届数" prop="AwdTerm">
                  <el-input v-model="detailFormBody.AwdTerm" :disabled="!ismodify" placeholder="请输入获奖届数（可空）" style="width:300px" ></el-input>
                </el-form-item>                
                <el-form-item label="获奖日期" prop="AwdTime"  >
                  <el-date-picker v-model="detailFormBody.AwdTime" :disabled="!ismodify" placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
                </el-form-item>
                <el-form-item label="项目名称" prop="AwdProName">
                  <el-input v-model="detailFormBody.AwdProName" :disabled="!ismodify" placeholder="请输入获奖项目名" style="width:300px" ></el-input>
                </el-form-item>
                <el-form-item v-if="!ismodify" label="项目所属学院" prop="AwdOrgName">
                  <el-input :value="detailFormBody.AwdOrgName" :disabled="!ismodify" style="width:300px" ></el-input>
                </el-form-item>                  
                <el-form-item  v-if="ismodify"  label="项目所属学院" prop="OrgID">
                  <el-select v-model="detailFormBody.OrgID" placeholder="请选择所属学院" style="width:300px">
                    <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item label="是否属于团队" prop="IsTeam">
                    <el-radio-group v-model="detailFormBody.IsTeam" :disabled="!ismodify" size="medium">
                        <el-radio border label="1">是</el-radio> 
                        <el-radio border label="0">否</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="新增人员" v-if="ismodify" >
                    <el-button type="info" @click="addMember" round  >新增成员</el-button>
                    <el-button type="info" @click="addTeacher" round  >新增教师</el-button>
                </el-form-item> 
                <!-- 新增学生信息 -->
                <div v-if="ismodify"> 
                  <div  v-for="(member,index) in detailFormBody.Members" :key="member.key">
                    <el-form-item  :label="'成员'+'【'+index+'】'">
                    </el-form-item>
                    <el-form-item  label="学号">
                      <el-input v-model="member.MemberID"  placeholder="请输入学号" style="width:300px; margin:0 10px 10px 0"></el-input>          
                    </el-form-item>
                    <el-form-item  label="姓名">             
                      <el-input v-model="member.MemberName" placeholder="请输入姓名" style="width:300px; margin:0 300px 10px 0"></el-input>
                    </el-form-item>
                    <el-form-item  label="姓名">  
                      <el-select v-model="member.OrgID" :placeholder="member.MemberOrgName" style="width:300px; margin:0 10px 10px 0">
                        <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
                      </el-select>
                    </el-form-item>
                    <el-form-item  label="姓名">  
                      <el-input v-model="member.MemberBranch" placeholder="请输入团支部" style="width:300px" >
                      </el-input>
                    </el-form-item>
                    <el-form-item  label="操作">  
                      <el-button type="danger" @click.prevent="removeMember(member)">删除成员</el-button>
                    </el-form-item>
                </el-form-item> 
                </div> 
              </div>                                                                                      
                <el-form-item v-if="!ismodify" label="负责人姓名" prop="AwdeeName">
                  <el-input v-model="detailFormBody.AwdeeName" :disabled="!ismodify" placeholder="请输入获奖人姓名" style="width:300px" ></el-input>
                </el-form-item> 
                <el-form-item v-if="!ismodify" label="负责人学院" prop="AwdeeOrgName">
                  <el-input v-model="detailFormBody.AwdeeOrgName" :disabled="!ismodify"  style="width:300px" ></el-input>
                </el-form-item>  
                <el-form-item v-if="!ismodify" label="负责人团支部" prop="AwdeeBranch">
                  <el-input v-model="detailFormBody.AwdeeBranch" :disabled="!ismodify"  style="width:300px" ></el-input>
                </el-form-item>
                <div v-if="!ismodify">
                <el-form-item v-for="(teacher,index) in Teachers" :key="teacher"  :label="'指导教师'+'【'+index+'】'" prop="AwdeeBranch">
                  <el-input :value="teacher" :disabled="!ismodify" placeholder=""></el-input>          
                </el-form-item> 
                </div>                                                  
                <el-form-item v-if="ismodify" label="上传图片" prop="FileUrl">
                  <el-upload action="http://upload.qiniu.com/"  :data="postData" :on-success="successUpload" :before-upload="beforePicUpload" >
                    <el-button size="small" type="primary">点击上传</el-button>
                    <!-- <div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div> -->
                  </el-upload>
                </el-form-item>                   
                <el-form-item v-if="ismodify" label="">                  
                    <el-button type="primary" @click.native="modifySubmit" >提交</el-button>            
                </el-form-item>                                  
            </el-form> 
          </div>  
          <div class="check-box">
           <el-form :model="detailFormBody" label-width="100px" ref="modifyFrom" :rules="rules" auto >            
              <el-form-item v-if="!ismodify" label="填报人" prop="HnrName" >
                <el-input v-model="detailFormBody.ApplyAccountName" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item>   
              <el-form-item v-if="!ismodify" label="填报人单位" prop="HnrName" >
                <el-input v-model="detailFormBody.ApplyAccountOrg" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item> 
              <el-form-item v-if="!ismodify" label="填报人角色" prop="HnrName" >
                <el-input v-model="detailFormBody.ApplyAccountRole" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item>
              <el-form-item v-if="!ismodify" label="填报时间" prop="HnrName" >
                <el-input v-model="detailFormBody.ApplyTime" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item> 
              <el-form-item v-if="!ismodify" label="审核状态" prop="State" >
                <el-input v-model="detailFormBody.State" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item>               
              <el-form-item  label="证明照片">
                <img class="file" src="http://oyzg731sy.bkt.clouddn.com/FlL70dFa87VxKgNSYDJ3AQcfCUr_" alt="暂无证明材料">
              </el-form-item>                                                           
           </el-form>
          </div>                     
        </div>
    </div>
    <el-dialog title="查看团队成员" :visible.sync="teamFormVisible" v-loading="teamLoading">
      <div class="members">
      <div class="member-info" v-for="(member,index) in detailFormBody.Members" :key="index">
        <label>成员【{{index}}】</label>
        <div class="single-info">
          <div class="info">
            <span class="info-laber">学号</span>
            <el-input v-model="member.MemberID" :disabled="!ismodify" style="width:300px;"></el-input>  
          </div>
          <div class="info">
            <span>姓名</span>
            <el-input v-model="member.MemberName" :disabled="!ismodify" style="width:300px;"></el-input>  
          </div> 
          <div class="info">
            <span>学院</span>
            <el-input v-model="member.MemberOrgName" :disabled="!ismodify" style="width:300px;"></el-input>  
          </div>  
          <div class="info">
            <span>团支部</span>
            <el-input v-model="member.MemberBranch" :disabled="!ismodify" style="width:300px;"></el-input>  
          </div>                                           
        </div>    
      </div>
      </div>
    </el-dialog>
</div>
</template>

<script type="text/ecmascript-6">
import {
  reqGetAwdList,
  reqGetOrgList,
  posRecordHonor,
  reqGetTeam
} from "../../api/api";
import PubMethod from "../../common/util";

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
      // 是否处于编辑状态
      ismodify: false,
      // 是否需要查看团队
      isCheckTeam: false,
      // 七牛云令牌
      postData: {
        token: this.$store.state.uploadToken
      },
      // 填充奖项数据
      AwardData: [],
      // 填充组织单位
      OrgData: [],
      // 指导教师数组
      Teachers: "",
      // 用户令牌
      access_token: "",
      // 详情、编辑表单相关数据
      detailFormBody: [],
      submitLoading: false,
      // 团队成员表单信息
      teamFormVisible: false,
      teamLoading: false,
      teamMembers: [],
      // 表单验证规则
      rules: {
        HonorID: { required: true, message: "请选择荣誉项目", trigger: "blur" },
        Annual: { required: true, message: "请选择获得年度", trigger: "blur" },
        HnrTime: { required: true, message: "请选择获得年月", trigger: "blur" },
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
  created() {
    this.detailFormBody = this.$store.state.singleAward;
    this.detailFormBody.State = PubMethod.transfRecordState(
      this.detailFormBody
    );
    this.detailFormBody.Grade = PubMethod.transfGrande(this.detailFormBody);
    if (this.$route.params.id == "modify") this.ismodify = true;
    this.Teachers = this.detailFormBody.Teacher.split("#");
    //this.detailFormBody.Members=this.teamMembers
    console.log(this.detailFormBody);
    // console.log(this.Teachers);
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
    // 转换新增选项中的获奖级别
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
      this.submitLoading = true;
      let param = {
        access_token: "11"
      };
      reqGetOrgList(param)
        .then(res => {
          this.submitLoading = false;
          this.OrgData = res.data.data.list;
        })
        .catch(res => {
          console.log(res);
        });
    },
    // 显示团队信息
    showTeam() {
      this.teamFormVisible = true;
      this.teamLoading = true;
      this.getTeam();
    },
    // 填充团队信息
    getTeam() {
      this.teamMembers = [];
      let param = {
        AwdRecordID: this.detailFormBody.AwdRecordID
      };
      param.access_token = "11";
      reqGetTeam(param)
        .then(res => {
          let teamMembers = res.data.data.Members;
          teamMembers.forEach(memberInfo => {
            this.teamMembers.push(memberInfo);
          });
          this.detailFormBody.Members = this.teamMembers;
          this.teamLoading = false;
        })
        .catch(res => {
          console.log(res);
        });
    },
    // 点击编辑后初始化
    selectModify() {
      if (
        this.detailFormBody.State == "待审核" ||
        this.detailFormBody.State == "已驳回"
      ) {
        this.ismodify = true;
        this.getOrg();
        this.getAward();
        this.getTeam();
        console.log(this.detailFormBody.Members);
      } else {
        this.$message({
          type: "error",
          message: "已审核记录不可再次编辑"
        });
      }
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
    // 提交编辑
    modifySubmit() {
      this.$refs["modifyFrom"].validate(valid => {
        if (valid) {
          this.submitLoading = true;
          //复制字符串
          let para = Object.assign({}, this.detailFormBody);
          para.Branch = para.Branch + "团支部";
          para.access_token = "terry";
          posModifyRecordHonor(para).then(res => {
            this.submitLoading = false;
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this, res.data);
            this.$refs["modifyFrom"].resetFields();
            //this.backToMain();
          });
        }
      });
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
      this.detailFormBody.FileUrl = this.$store.state.uploadUrl + res.key;
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
  > .modify-box {
    margin: 0 10%;
    //flex:auto;
  }
  > .check-box {
    //flex:auto;
    .file {
      width: 200px;
      height: 200px;
    }
  }
}
.members {
  .member-info {
    display: flex;
    flex-direction: column;
    align-items: center;
    .single-info {
      width: 100%;
      .info {
        display: flex;
        justify-content: center;
        position: relative;
        span {
          position: absolute;
          left: 10%;
        }
      }
    }
  }
}
</style>
