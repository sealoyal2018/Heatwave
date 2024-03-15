import request from '@/utils/request'

// 登录方法
export function login(username, password, code, uuid) {
  const data = {
    username,
    password,
    code,
    uuid
  }
  return request({
    url: '/account/login',
    headers: {
      isToken: false
    },
    method: 'post',
    data: data
  })
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
  return request({
    url: '/account',
    method: 'get'
  })
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

  return request({
    url: '/api/Auth/Captche',
    headers: {
      isToken: false
    },
    method: 'get',
    timeout: 20000
  })
}

export async function getTenantByUserName(username){

  const res = await request({
    url: '/api/auth/Tenant?username='+username,
    method: 'get',
    headers: {
      isToken: false,
    },
    timeout: 2000
  });
  return res.data;
}