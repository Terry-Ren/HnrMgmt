// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
// 引用elementUI
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-default/index.css'
// 引入axios
import axios from 'axios'
import App from './App'
import router from './router'

Vue.use(ElementUI)
// 通过$http调用axios
Vue.prototype.$http = axios

Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
