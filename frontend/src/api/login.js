import request from '@/utils/request'
import {get, post} from '@/utils/http'

// 登录方法
export function login(data) {
  return post('/api/auth/token',data, false);
}

// 注册方法
export function register(data) {
  return request({
    url: '/register',
    headers: {
      isToken: false
    },
    method: 'post',
    data: data
  })
}

// 获取用户详细信息
export function getInfo() {
  return get('/api/user/info')
}

// 退出方法
export function logout() {
  return request({
    url: '/account/logout',
    method: 'post'
  })
}

// 获取验证码
export function getCodeImg() {
  return get('/api/Auth/Captche');
}

export function getTenantByUserName(username){
  return get('/api/auth/Tenant', {username: username})
}