import Vue from 'vue'
import Router from 'vue-router'
import Home from '../views/Home.vue'
// import Hello from '@/components/Hello'

Vue.use(Router)

// 定义路由
const routes = [{
  path: '/',
  redirect: {
    name: 'Home'
  }
}, {
  path: '/Home',
  name: 'Home',
  component: Home
}
]

// 暴露路由
export default new Router({
  routes: routes
})

// export default new Router({
//   routes: [
//     {
//       path: '/',
//       name: 'Hello',
//       component: Hello
//     }
//   ]
// })
