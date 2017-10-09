import Vuex from 'vuex'
import Vue from 'vue'
import * as types from './mutation-types.js'

// 调用
Vue.use(Vuex)

export default new Vuex.Store({
    state:{
        access_token:'',
        RoleID:''
    },
    mutations:{
        // 登录时写入
        [types.LOGIN]:(state,data)=>{
            localStorage.access_token=data.Password
            localStorage.RoleID=data.RoleID
            state.access_token=data.Password
            state.RoleID=data.RoleID
        },
        // 刷新时重写
        [types.REFREST]:(state)=>{
            state.access_token=localStorage.access_token,
            state.RoleID=localStorage.RoleID
        }
    }
})