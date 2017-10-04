import axios from 'axios'

export const reqGetBookListPage = params => { return axios.get(`http://211.149.193.19:8080/api/customers`) }
