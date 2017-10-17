<template>
<el-row>
  <el-col :span="24">
<h3> {{message}} </h3>
<el-button type="infor"  @click.prevent="update"> hahah </el-button>
  </el-col>
</el-row>

</template>

<script type="text/ecmascript-6">
 export default {
   data() {
     return {
         message:'更新前'
     }

   },
   methods:{
       update(){
                 this.message = '更新完成'
      console.log(this.$el.textContent) // => '没有更新'
      this.$nextTick(function () {
        console.log(this.$el.textContent) // => '更新完成'
      })
       }
   }
 }
</script>

<style scoped lang="scss">

 
</style>
