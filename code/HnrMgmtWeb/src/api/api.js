import axios from 'axios'
import store from '../store/index'
import * as types from '../store/mutation-types'
import router from '../router/index'

// http response 拦截器
axios.interceptors.response.use(
    response => {
      return response
    },
    error => {
      if (error.response) {
        switch (error.response.status) {
          case 401:
          // 返回 401 清除token信息并跳转到登录页面
            store.commit(types.LOGOUT)
            router.replace({
              path: '/login',
              query: {redirect: router.currentRoute.fullPath}
            })
        }
      }
      return Promise.reject(error.response.data)   // 返回接口返回的错误信息
    })

let base = 'http://localhost:59996/'

// 请求荣誉列表
export const reqGetHonorList = params => { return axios.get(`${base}api/honor/get?access_token=11`, { params: params }) }
// 添加荣誉项
export const reqAddHonor = params => { return axios.get(`${base}api/honor/add?access_token=11`, { params: params }) }
// 修改荣誉项
export const posModifyHonor = params => { return axios.post(`${base}api/honor/modify`, params) }
// 删除一项荣誉
export const reqDeleteHonor = params => { return axios.get(`${base}api/honor/delete`, { params: params }) }

// 请求奖项列表
export const reqGetAwdList = params => { return axios.get(`${base}api/award/get`, {params: params}) }
// 添加奖项
export const posAddAwd = params => { return axios.post(`${base}api/award/add`, params) }
// 修改奖项
export const posModifyAwd = params => { return axios.post(`${base}api/award/modify`, params) }
// 删除奖项
export const reqDeleteAwd = params => { return axios.get(`${base}api/award/delete`, {params: params}) }

// 请求单位列表
export const reqGetOrgList = params => { return axios.get(`${base}api/org/get`, {params: params}) }
// 添加单位
export const posAddOrg = params => { return axios.post(`${base}api/org/add`, params) }
// 修改单位
export const posModifyOrg = params => { return axios.post(`${base}api/org/modify`, params) }
// 删除单位
export const reqDeleteOrg = params => { return axios.get(`${base}api/org/delete`, {params: params}) }

// 新增三级管理员
export const reqGetAccTchList = params => { return axios.get(`${base}api/account/teacher`, {params: params}) }
