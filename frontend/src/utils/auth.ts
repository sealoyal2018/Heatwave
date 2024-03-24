import Cookies from "js-cookie";
import { storageLocal } from "@pureadmin/utils";
import { UserToken } from "@/api/auth";

export const userKey = "user-info";
export const tokenKey = "authorized-token";
export const refreshTokenKey = "authorized-refresh-token";
/**
 * 通过`multiple-tabs`是否在`cookie`中，判断用户是否已经登录系统，
 * 从而支持多标签页打开已经登录的系统后无需再登录。
 * 浏览器完全关闭后`multiple-tabs`将自动从`cookie`中销毁，
 * 再次打开浏览器需要重新登录系统
 * */
export const multipleTabsKey = "multiple-tabs";

/** 获取`token` */
export function getToken(): { accessToken, expires } {
  // 此处与`TokenKey`相同，此写法解决初始化时`Cookies`中不存在`TokenKey`报错
  return Cookies.get(tokenKey)
    ? JSON.parse(Cookies.get(tokenKey))
    : storageLocal().getItem(tokenKey);
}

export function getRefreshToken(): { refreshToken, expires } {
  // 此处与`TokenKey`相同，此写法解决初始化时`Cookies`中不存在`TokenKey`报错
  const token = Cookies.get(refreshTokenKey);
  return Cookies.get(refreshTokenKey)
    ? JSON.parse(Cookies.get(refreshTokenKey))
    : storageLocal().getItem(refreshTokenKey);
}


/**
 * @description 设置`token`以及一些必要信息并采用无感刷新`token`方案
 * 无感刷新：后端返回`accessToken`（访问接口使用的`token`）、`refreshToken`（用于调用刷新`accessToken`的接口时所需的`token`，`refreshToken`的过期时间（比如30天）应大于`accessToken`的过期时间（比如2小时））、`expires`（`accessToken`的过期时间）
 * 将`accessToken`、`expires`这两条信息放在key值为authorized-token的cookie里（过期自动销毁）
 * 将`username`、`roles`、`refreshToken`、`expires`这四条信息放在key值为`user-info`的localStorage里（利用`multipleTabsKey`当浏览器完全关闭后自动销毁）
 */
export function setToken(data: UserToken) {
  const { accessToken, refreshToken } = data;
  let expires = new Date(data.accessTokenExpires).getTime(); // 如果后端直接设置时间戳，将此处代码改为expires = data.expires，然后把上面的DataInfo<Date>改成DataInfo<number>即可
  let refreshExpires = new Date(data.refreshTokenExpires).getTime();
  const cookieString = JSON.stringify({ accessToken, expires });
  const refCookiesString = JSON.stringify({ refreshToken, refreshExpires });

  expires > 0
    ? Cookies.set(tokenKey, cookieString, {
      expires: (expires - Date.now()) / 86400000
    })
    : Cookies.set(tokenKey, cookieString);

  refreshExpires > 0
    ? Cookies.set(refreshTokenKey, refCookiesString, { expires: (refreshExpires - Date.now()) / 86400000 })
    : Cookies.set(refreshTokenKey, refCookiesString);


  Cookies.set(
    multipleTabsKey,
    "true", { expires: (refreshExpires - Date.now()) / 86400000 }
  );
}

/** 删除`token`以及key值为`user-info`的localStorage信息 */
export function removeToken() {
  Cookies.remove(tokenKey);
  Cookies.remove(refreshTokenKey);
  Cookies.remove(multipleTabsKey);
  storageLocal().removeItem(userKey);
}

/** 格式化token（jwt格式） */
export const formatToken = (token: string): string => {
  return token;
};
