import Vue from 'vue'
import Router from 'vue-router'
import Home from '../views/Home.vue'
import NotFoundComponent from '../views/404'
import Hello from '@/components/Hello123'
import Hnrlist from '../components/BasicData/Hnrlist.vue'
import Awdlist from '../components/BasicData/Awdlist.vue'
import Orglist from '../components/BasicData/Orglist.vue'

Vue.use(Router)

// 定义路由
const routes = [
//   {

//   // path: '/',
//   // redirect: {
//   //   name: 'Home'
//   // }
// },
// 404页面
  {
    path: '*',
    component: NotFoundComponent
  },
  // 首页
  {
    path: '/',
    name: 'Home',
    component: Home,
    redirect: '/index',
    children: [
      {path: '/index', component: Hello, name: 'index', menuShow: true},
    ]
  },
  // 基础数据
  {
    path: '/',
    component: Home,
    name: 'BasicData',
    menuShow: true,
    // iconCls: 'iconfont icon-users', // 图标样式class
    children: [
      {path: '/basic/hnrlist', component: Hnrlist, name: 'Hnrlist', menuShow: true},
      {path: '/basic/awdlist', component: Awdlist, name: 'Awdlist', menuShow: true},
      {path: '/basic/orglist', component: Orglist, name: 'Orglist', menuShow: true}
    ]
  },
  // 系统管理
  {
    path: '/',
    component: Home,
    name: 'SystemData',
    menuShow: true,
    children: [
      {path: '/system/acclist', component: Hnrlist, name: 'Hnrlist', menuShow: true},
    ]

  }
]

// 暴露路由
export default new Router({
  mode: 'history',
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
