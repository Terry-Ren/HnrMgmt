<template>
<div class="container">
    <!-- 面包屑导航 -->
    <div class="warp-breadcrum">
        <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/' }"><b>首页</b></el-breadcrumb-item>
            <el-breadcrumb-item>记录管理</el-breadcrumb-item>
            <el-breadcrumb-item :to="{ path: '/record/honor' }">荣誉管理</el-breadcrumb-item>
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
            <el-button v-if="!ismodify" type="primary" @click="review" >审核</el-button> 
            <el-button  v-if="!ismodify" type="primary" @click="rejectFormVisible=true;">驳回</el-button>                         
            <el-button type="infor" @click="backToMain" >返回</el-button>            
            </el-form>             
        </div>
        <!-- 主要表单 -->
        <div class="main-data" v-loading="submitLoading">
          <div class="modify-box">
            <!-- 编辑表单 -->
            <el-form :model="detailFormBody" label-width="100px" ref="modifyFrom" :rules="rules" auto>
                <el-form-item v-if="!ismodify" label="荣誉项目" prop="HnrName" >
                  <el-input v-model="detailFormBody.HnrName" :disabled="!ismodify" style="width:300px" ></el-input>                                    
                </el-form-item>                
                <el-form-item v-if="ismodify" label="荣誉项目" prop="HonorID">
                  <el-select v-model="detailFormBody.HonorID" placeholder="请选择荣誉" style="width:300px">
                    <el-option v-for="honor in HonorData" :key="honor.HonorID" :value="honor.HonorID" :label="honor.Name"></el-option>
                  </el-select>          
                </el-form-item> 
                <el-form-item  label="获奖年度" prop="HnrAnnual" >
                  <el-select v-model="detailFormBody.HnrAnnual" :disabled="!ismodify"  placeholder="请选择年度" style="width:300px">
                    <el-option v-for="options in annualOptions" :key="options.value" :label="options.label" :value="options.value"></el-option>
                  </el-select>                  
                </el-form-item>
                <el-form-item label="获奖日期" prop="HnrTime"  >
                <el-date-picker v-model="detailFormBody.HnrTime" :disabled="!ismodify" placeholder="获得年月" style="width:300px" format="yyyy 年 MM 月" value-format="yyyy-MM"></el-date-picker>
                </el-form-item>
                <el-form-item label="获奖人学号" prop="AwdeeID">
                <el-input v-model="detailFormBody.AwdeeID" :disabled="!ismodify" placeholder="请输入获奖人学号" style="width:300px" ></el-input>
                </el-form-item>          
                <el-form-item label="获奖人姓名" prop="AwdeeName">
                <el-input v-model="detailFormBody.AwdeeName" :disabled="!ismodify" placeholder="请输入获奖人姓名" style="width:300px" ></el-input>
                </el-form-item> 
                <el-form-item v-if="!ismodify" label="单位学院" prop="AwdeeOrgName">
                <el-input v-model="detailFormBody.AwdeeOrgName" :disabled="!ismodify"  style="width:300px" ></el-input>
                </el-form-item>                 
                <el-form-item v-if="ismodify" label="单位学院" prop="OrgID">
                <el-select v-model="detailFormBody.OrgID" placeholder="请选择所属学院" style="width:300px">
                    <el-option v-for="org in OrgData" :key="org.OrgID" :value="org.OrgID" :label="org.Name"></el-option>
                </el-select>
                </el-form-item> 
                <el-form-item label="所属团支部" prop="AwdeeBranch">
                <el-input v-model="detailFormBody.AwdeeBranch" :disabled="!ismodify" placeholder="请输入团支部" style="width:300px" >
                    <template slot="append">团支部</template>
                </el-input>
                </el-form-item>
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
              <el-form-item v-if="!ismodify" label="审核状态" prop="StateInfo" >
                <el-input v-model="detailFormBody.StateInfo" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item>  
              <el-form-item v-if="!ismodify&&!(detailFormBody.RejectReason==null)" label="驳回原因" prop="RejectReason" >
                <el-input v-model="detailFormBody.RejectReason" :disabled="!ismodify" style="width:300px" ></el-input>                                    
              </el-form-item>                            
              <el-form-item  label="证明照片">
                <img class="file" :src="detailFormBody.FileUrl" alt="暂无证明材料">
              </el-form-item>                                                           
           </el-form>
          </div>                     
        </div>
    </div>

    <!-- 驳回请求表单      -->
    <el-dialog title="驳回请求" :visible.sync="rejectFormVisible" v-loading="rejectLoading" style="width:70%;">
      <el-form  label-width="80px" ref="rejectFrom"  >
        <el-form-item label="驳回理由" prop="reson"  >
          <el-input v-model="RejectReason"  type="textarea" :autosize="{ minRows: 2, maxRows: 4}" placeholder="请输入驳回理由" style="width:300px" ></el-input>
        </el-form-item>  
      </el-form> 
      <div slot="footer" class="dialog-footer">
        <el-button @click.native=" rejectFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="rejectSubmit" >提交</el-button>
      </div>            
    </el-dialog>       
</div>
</template>

<script type="text/ecmascript-6">
import PubMethod from "../../common/util";
import * as types from "../../store/mutation-types";
import {
  reqGetHonorList,
  reqGetOrgList,
  posModifyRecordHonor,
  reqGetReviewRecord,
  reqGetRejectRecord  
} from "../../api/api";

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
      // 七牛云令牌
      postData: {
        token: this.$store.state.uploadToken
      },
      // 填充荣誉数据
      HonorData: [],
      // 填充组织单位
      OrgData: [],
      // 用户令牌
      access_token: "",
      // 新增表单相关数据
      detailFormBody: [],
      submitLoading: false,
      //拒绝原因填写
      rejectFormVisible: false,
      rejectLoading: false,
      RejectReason: "",      
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
        AwdeeBranch: { required: true, message: "请输入所属团支部", trigger: "blur" }
      },
      // 获奖年度选择
      annualOptions: [
        {
          value: "2013-2014",
          label: "2013-2014"
        },
        {
          value: "2014-2015",
          label: "2014-2015"
        },
        {
          value: "2015-2016",
          label: "2015-2016"
        }
      ]
    };
  },
  created() {
    this.detailFormBody = this.$store.state.singleHonor;
    this.detailFormBody.StateInfo = PubMethod.transfRecordState(
      this.detailFormBody
    );
    if (this.$route.params.id == "modify") this.selectModify();
  },
  methods: {
    // 跳转路由
    backToMain() {
      this.$router.push({
        path: "/record/honor"
      });
    },
    // 填充荣誉数据
    getHonor() {
      this.submitLoading = true;
      let param = {
        access_token: "11"
      };
      reqGetHonorList(param)
        .then(res => {
          this.submitLoading = false;
          this.HonorData = res.data.data.list;
        })
        .catch(res => {
          console.log(res);
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
    // 点击编辑后初始化
    selectModify() {
      if (
        this.detailFormBody.StateInfo == "待审核" ||
        this.detailFormBody.StateInfo == "已驳回"
      ) {
        this.ismodify = true;
        this.getOrg();
        this.getHonor();
      } else {
        this.$message({
          type: "error",
          message: "已审核记录不可再次编辑"
        });
      }
    },
    // 提交编辑
    modifySubmit() {
      this.$refs["modifyFrom"].validate(valid => {
        if (valid) {
          this.submitLoading = true;
          //复制字符串
          let para = Object.assign({}, this.detailFormBody);
          para.Branch = para.AwdeeBranch + "团支部";
          para.access_token = "11";
          posModifyRecordHonor(para).then(res => {
            this.submitLoading = false;
            //公共提示方法，传入当前的vue以及res.data
            PubMethod.statusinfo(this, res.data);
            this.$refs["modifyFrom"].resetFields();
            this.$store.commit(types.RECORD_MODIFY)
            this.backToMain();
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
    },
    // 审核通过
    review() {
      this.$confirm("是否审核通过该记录?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          this.submitLoading = true;
          let param = {
            access_token: "11",
            HnrRecordID: this.detailFormBody.HnrRecordID
          };
          reqGetReviewRecord(param).then(res => {
            this.submitLoading = false;
            PubMethod.statusinfo(this, res.data);
            this.$store.commit(types.RECORD_MODIFY);
            this.detailFormBody.StateInfo = '已审核';
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除"
          });
        });
    },
    // 驳回
    rejectSubmit() {
      //this.$refs["rejectFrom"].validate(valid => {
      // if (valid) {
      this.rejectLoading = true;
      //复制字符串
      let param = {
        access_token: "11",
        HnrRecordID: this.detailFormBody.HnrRecordID,
        RejectReason: this.RejectReason
      };
      reqGetRejectRecord(param).then(res => {
        this.rejectLoading = false;
        //公共提示方法，传入当前的vue以及res.data
        PubMethod.statusinfo(this, res.data);
        this.$refs["rejectFrom"].resetFields();
        this.rejectFormVisible = false;
        this.$store.commit(types.RECORD_MODIFY);
        this.detailFormBody.StateInfo = '已驳回';
      });
      //  }
      // });
    }    
  }
};
</script>

<style scoped lang="scss">
.toolbar{
  form{
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
</style>
