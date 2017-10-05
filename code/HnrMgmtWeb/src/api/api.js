import axios from 'axios'

let base = 'http://localhost:59996/'

// 请求荣誉列表
export const reqGetHonorList = params => { return axios.get(`${base}api/honor/get?access_token=11`) }
// 添加荣誉项
export const reqAddHonor = params => { return axios.get(`${base}api/honor/add?access_token=11`, { params: params }) }
// 修改荣誉项
export const posModifyHoenr = params => { return axios.post(`${base}api/honor/modify`, params) }
// 删除一项荣誉
export const reqDeleteHonor = params => { return axios.get(`${base}api/honor/delete`, { params: params }) }
