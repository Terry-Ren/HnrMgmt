import Vue from 'vue'
import Router from 'vue-router'
import store from '../store/index'
import * as types from '../store/mutation-types'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'
import NotFoundComponent from '../views/404'
import Hello from '@/components/Hello123'
import RecHonor from '../components/RecordData/RecHonor'
import RecAward from '../components/RecordData/RecAward'
import AddAward from '../components/RecordData/AddAward'
import AddHonor from '../components/RecordData/AddHonor'

import TEXT from '../views/Home1.vue'


// import text from '../components/BasicData/text.vue'
// import AccTchlist from '../components/SystemData/AccTchlist.vue'

// 将基础数据组件全部打包异步加载（webpack特殊的注释语法）
const Hnrlist = () => import(/* webpackChunkName: "BasicData" */ '../components/BasicData/Hnrlist.vue')
const Awdlist = () => import(/* webpackChunkName: "BasicData" */ '../components/BasicData/Awdlist.vue')
const Orglist = () => import(/* webpackChunkName: "BasicData" */ '../components/BasicData/Orglist.vue')

// 系统管理组件打包异步
const ComAcclist = () => import(/* webpackChunkName: "SystemData" */ '../components/SystemData/ComAcclist')
const Rolelist = () => import(/* webpackChunkName: "SystemData" */ '../components/SystemData/Rolelist')
const ChangePass = () => import(/* webpackChunkName: "SystemData" */ '../components/SystemData/Password')
const Menulist = () => import(/* webpackChunkName: "SystemData" */ '../components/SystemData/Menulist')

Vue.use(Router)

// 定义路由数据
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
    name: 'Error',
    component: NotFoundComponent
  },
  // 测试页
        {path:'/text',component:TEXT,name:'TEXT',menuShow:true},
  // 登录页
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  // 首页
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: {
      requireAuth: true,  
      keepAlive:false
    },
    redirect: '/index',
    children: [
      {path: '/index', component: Hello, name: 'index', menuShow: true},
    ]
  },
  // 记录填报
  {
    path: '/',
    component: Home,
    name: 'RecordData',
    meta: {
      requireAuth: true,
      keepAlive:true,
    },
    menuShow: true,
    children: [
      { path: '/record/honor', component: RecHonor, name: 'RecHonor',meta:{keepAlive:true}, menuShow: true },
      { path: '/record/award', component: RecAward, name: 'RecAward', meta: { keepAlive: true }, menuShow: true },
      { path: '/record/addaward', component: AddAward, name: 'AddAward', menuShow: true },     
      { path: '/record/addhonor', component: AddHonor, name:'AddHonor',menuShow:true}      
    ]
  },
  // 基础数据
  {
    path: '/',
    component: Home,
    name: 'BasicData',
    meta: {
      requireAuth: true,
      keepAlive: false
    },
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
    meta: {
      requireAuth: true,
      keepAlive: false
      
    },
    menuShow: true,
    children: [
      {path: '/system/Acclist', component: ComAcclist, name: 'ComAcclist', menuShow: true},
      {path: '/system/Role', component: Rolelist, name: 'Rolelist', menuShow: true},
      {path: '/system/PassWord', component: ChangePass, name: 'ChangePass', menuShow: true},
      { path: '/system/Menulist', component: Menulist, name: 'Menulist', menuShow: true }
    ]

  }
]

// 页面刷新时，重新赋值给state中相应的变量
if (window.localStorage.getItem('access_token')) {
  store.commit(types.REFREST)
}

// 声明路由
const router = new Router({
  mode: 'history',
  routes: routes
})

// 导航守卫
router.beforeEach((to, from, next) => {
  if (to.matched.some(r => r.meta.requireAuth)) {
    if (store.state.access_token) {
      next()
    } else {
      next({
        path: '/login',
        query: {redirect: to.fullPath}
      })
    }
  } else {
    next()
  }
})
// 暴露路由
export default router

// export default new Router({
//   routes: [
//     {
//       path: '/',
//       name: 'Hello',
//       component: Hello
//     }
//   ]
// })
