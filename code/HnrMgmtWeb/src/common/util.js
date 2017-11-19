export default {
    //测试  Console.Log
    logMessage:message=>{
        console.log(message);
    },
    //进行奖项荣誉级别转换
    transfGrandeName(row){
       return row.GradeName == '0' ? '院级' : row.GradeName == '1' ? '校级' : row.GradeName == '2' ? '省级': '国级';
     },
     //进项奖项等次转换
     transfGrande(row){
        return row.Grade == '0' ? '三等奖' : row.Grade == '1' ? '二等奖' : row.Grade == '2' ? '一等奖': '特等奖';
      },
     // 账号状态转换
     transfState(row){
       return row.State=='0' ? '冻结' : '正常';
     },
     // 审核状态转换
     transfRecordState(row){
       return row.State=='0' ? '待审核' : row.State == '1' ? '院审通过' : row.State == '2' ? '校审通过' : '已驳回';
     },
      //公共提示方法，传入当前的vue以及res.data。 返回状态判断并给出提示
      statusinfo(vue,data){
        if(data.status=='success'){
            vue.$message({
            type: 'success',
            message: data.messages
          })}else{ 
            vue.$message({
            type: 'error',
            message: data.messages
          })             
          }
      }

}
