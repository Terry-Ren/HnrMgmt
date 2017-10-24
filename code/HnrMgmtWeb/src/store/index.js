import Vuex from 'vuex'
import Vue from 'vue'
import * as types from './mutation-types.js'

// 调用
Vue.use(Vuex)

export default new Vuex.Store({
    state:{
        access_token:'',
        RoleID:'',
        Name:'',
        ID:''
    },
    mutations:{
        // 登录时写入
        [types.LOGIN]:(state,resData)=>{
            localStorage.access_token=resData.data.access_token
            localStorage.RoleID=resData.data.RoleID
            localStorage.Name=resData.data.Name
            localStorage.ID=resData.data.ID
            state.access_token=resData.data.access_token
            state.RoleID=resData.data.RoleID
            state.Name=resData.data.Name
            state.ID=resData.data.ID
        },
        // 登出时清空
        [types.LOGOUT]:(state)=>{
            localStorage.access_token=''
            localStorage.RoleID=''
            localStorage.Name=''
            localStorage.ID=''
            state.access_token=''
            state.RoleID=''
            state.Name=''
            state.ID=''
        },
        // 刷新时重写
        [types.REFREST]:(state)=>{
            state.access_token=localStorage.access_token,
            state.RoleID=localStorage.RoleID
            state.Name=localStorage.Name
            state.ID=localStorage.ID
        },
        // 401时
        [types.DENY]:(state)=>{
            state.access_token=''
        }
    }
})


