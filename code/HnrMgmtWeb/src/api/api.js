import axios from 'axios'

let baseUrl = 'http://localhost:59996/'

export const reqGetBookListPage = params => { return axios.get(`${baseUrl}api/customers`) }

// get请求参数格式写法
export const reqGetAwdListPage = params => { return axios.get(`${baseUrl}api/award/get`, {params: params}) }
