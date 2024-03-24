import * as http from "@/api/base";

export type CaptchaResult = {
  captcha: string;
  key: string;
  expireTime: any;
}

export interface UserInfo {
  nickname: string;
  avatar: string;
  email?: string;
  phone?: string;
  permissions?: string[];
}

export type UserToken = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpires: Date;
  refreshTokenExpires: Date;
}

export function getCaptcha(): Promise<http.Result<CaptchaResult>> {
  return http.get("/api/auth/captcha");
}

/** 登录 */
export function login(data?: object): Promise<http.Result<UserToken>> {
  return http.post("/api/auth/token", data);
};

/** 刷新token */
export function refreshToken(data?: object): Promise<http.Result<UserToken>> {
  return http.post("/api/auth/refreshtoken", { data });
};

/** 获取用户信息 */
export function getUserInfo(): Promise<http.Result<UserInfo>> {
  return http.get('/api/auth/info');
}

/** 注销 */
export const logout = () => {
  return http.get<any, any>('/api/auth/logout')
}

/** 获取当前用户的菜单 */
export const getRouters = () => {
  return http.get<any, any>('/api/auth/routes')
}

/** 获取租户 */
export function getTenants(username: string): Promise<http.Result<any>> {
  return http.get('/api/auth/tenant', { username });
}

