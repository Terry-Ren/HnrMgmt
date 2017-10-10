<template>
<component :is="componentId"></component>
</template>

<script type="text/ecmascript-6">
import AccTchlist from '../SystemData/AccTchlist'
import AccAdmlist from '../SystemData/AccAdmlist'
 export default {
   data() {
     return {
        componentId:''
     }
   },
   components: {
       AccTchlist,
       AccAdmlist,
   },
   beforeRouteEnter(to,from,next) {
       next(vm=>{
       if(vm.$store.state.RoleID=='0'){
       vm.componentId='AccTchlist'
       }
       else{
           vm.componentId='AccAdmlist'
       }
       })    
   }
 }
</script>
