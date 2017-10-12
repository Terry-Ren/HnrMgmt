import Vuex from 'vuex'
import Vue from 'vue'
import * as types from './mutation-types.js'

// 调用
Vue.use(Vuex)

export default new Vuex.Store({
    state:{
        access_token:'',
        RoleID:'',
        Name:''
    },
    mutations:{
        // 登录时写入
        [types.LOGIN]:(state,resData)=>{
            localStorage.access_token=resData.data.access_token
            localStorage.RoleID=resData.data.roleID
            localStorage.Name=resData.data.name
            state.access_token=resData.data.access_token
            state.RoleID=resData.data.roleID
            state.Name=resData.data.name
        },
        // 登出时清空
        [types.LOGOUT]:(state)=>{
            localStorage.access_token=''
            localStorage.RoleID=''
            localStorage.Name=''
            state.access_token=''
            state.RoleID=''
            state.Name=''
        },
        // 刷新时重写
        [types.REFREST]:(state)=>{
            state.access_token=localStorage.access_token,
            state.RoleID=localStorage.RoleID
            state.Name=localStorage.Name
        }
    }
})


