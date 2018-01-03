import axios from 'axios'
import store from '../store/index'
import * as types from '../store/mutation-types'
import router from '../router/index'

// http request 拦截器
// axios.interceptors.request.use(
//   config => {
//     if (store.state.access_token) {
//       config.headers.Authorization = `access_token ${store.state.access_token}`
//     }
//     return config
//   },
//   err => {
//     return Promise.reject(err)
//   })

// http response 拦截器
// axios.interceptors.response.use(
//     response => {
//       console.log(response)
//       return response
//     },
//     error => {
//        console.log(error)
      // if (error.response) {
      //   switch (error.response.status) {
      //     case 401:
      //     // 返回 401 清除token信息并跳转到登录页面
      //       console.log('error')
      //       store.commit(types.DENY)
      //       router.replace({
      //         path: '/login',
      //         query: {redirect: router.currentRoute.fullPath}
      //       })
      //   }
      // }
      // store.commit(types.DENY)
      // router.replace({
      //   path: '/login',
      //   query: {redirect: router.currentRoute.fullPath}
      // })
    //   return Promise.reject(error.response.data)   // 返回接口返回的错误信息
    // })

let base = 'http://localhost:59996/'

// 登录接口
export const posLogin = params => { return axios.post(`${base}api/auth/login`, params) }

// text
export const reqGetAwdListJava = params => { return axios.get(`http://localhost:8787/api/award/get`, { params: params }) }
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

// 获取三级管理员
export const reqGetAccTchList = params => { return axios.get(`${base}api/account/teacher`, {params: params}) }
// 新增三级管理员
export const posAccTch = params => { return axios.post(`${base}api/account/addteacher`, params) }
// 修改三级管理员
export const posModifyAccTch = params => { return axios.post(`${base}api/account/modteacher`, params) }
// 删除三级管理员
export const reqDeleteAccTch = params => { return axios.get(`${base}api/account/delteacher`, {params: params}) }
// 重置三级管理员密码
export const reqResetAccTch = params => { return axios.get(`${base}api/account/resetteacher`, {params: params}) }

// 获取助理人员
export const reqGetAccAdmList = params => { return axios.get(`${base}api/account/admin`, {params: params}) }
// 新增助理人员
export const posAccAdm = params => { return axios.post(`${base}api/account/addadmin`, params) }
// 修改助理人员
export const posModifyAccAdm = params => { return axios.post(`${base}api/account/modteacher`, params) }
// 删除助理人员
export const reqDeleteAccAdm = params => { return axios.get(`${base}api/account/deladmin`, {params: params}) }
// 重置助理人员密码
export const reqResetAccAdm = params => { return axios.get(`${base}api/account/resetadmin`, {params: params}) }
// 冻结助理人员
export const reqBlockAccAdm = params => { return axios.get(`${base}api/account/blockstate`, {params: params}) }

// 获取角色列表
export const reqGetRoleList = params => { return axios.get(`${base}api/role/getrole`, {params: params}) }
// 获取角色权限
export const reqGetRoleControl = params => { return axios.get(`${base}api/role/get`, {params: params}) }

// 修改密码
export const posModifyPass = params => { return axios.post(`${base}api/password/modify`, params) }

// 获取已记录接口
export const reqGetApiList = params => { return axios.get(`${base}api/role/getmenu`, {params: params}) }
// 新增接口记录
export const posAddApi = params => { return axios.post(`${base}api/role/addmenu`, params) }
// 修改接口信息
export const posModifyApi = params => { return axios.post(`${base}api/role/modmenu`, params) }
// 删除所选接口
export const reqDeleteApi = params => { return axios.get(`${base}api/role/delmenu`, {params: params}) }

// 申报荣誉记录
export const posRecordHonor = params => { return axios.post(`${base}api/record/honor`, params) }
// 获取荣誉或奖项记录
export const reqGetRecord = params => { return axios.get(`${base}api/record/get`, { params: params }) }
// 修改荣誉记录
export const posModifyRecordHonor = params => { return axios.post(`${base}api/record/hnrmodify`, params) }
// 删除荣誉记录
export const reqDeleteRecord = params => { return axios.get(`${base}api/record/delete`, { params: params }) }

// 申报奖项记录
export const posRecordAward = params => { return axios.post(`${base}api/record/award`, params) }
// 团队信息获取
export const reqGetTeam = params => { return axios.get(`${base}api/record/teaminfo`, { params: params }) }
// 修改奖项信息
export const posModifyRecordAward = params => { return axios.post(`${base}api/record/awdmodify`, params) }

// 个人信息获取
export const reqGetAccountInfo = params => { return axios.get(`${base}api/account/getinfo`, { params: params }) }
// 个人信息修改
export const posModifyAccountInfo = params => { return axios.post(`${base}api/account/changeinfo`, params) }

// 审核记录
export const reqGetReviewRecord = params => { return axios.get(`${base}api/record/auditpass`, { params: params }) }
// 驳回记录
export const reqGetRejectRecord = params => { return axios.get(`${base}api/record/auditreject`, { params: params }) }

//查询记录接口
export const queryDataInfo = params => { return axios.post(`${base}api/record/multiconditionquery`, params) }

