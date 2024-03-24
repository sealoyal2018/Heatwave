import { defineStore } from 'pinia';

import { store } from '..';
import * as authRequest from '@/api/auth';
import { storageLocal, storageSession } from '@pureadmin/utils';
import * as authCache from '@/utils/auth'
import auth from '@/components/ReAuth/src/auth';
import { Result } from '@/api/base';
import { routerArrays } from "@/layout/types";
import { message } from "@/utils/message";

import { useMultiTagsStoreHook } from "@/store/modules/multiTags";
import { resetRouter, router } from "@/router";

const authStore = defineStore({
  id: 'auth-store',
  state: () => ({
    accessToken: storageSession().getItem(authCache.tokenKey),
    refreshToken: storageSession().getItem(authCache.refreshTokenKey),
    userInfo: storageSession().getItem(authCache.userKey),
  }),
  getters: {
    getAccessToken(state): string | null {
      return state.accessToken;
    },
    getRefreshToken(state): string | null {
      return state.refreshToken;
    },
    getUserInfo(state): authRequest.UserInfo | undefined | null {
      return state.userInfo;
    },
  },
  actions: {
    SET_ACCESS_TOKEN(accessToken: string) {
      this.accessToken = accessToken;
    },
    SET_REFHRESH_TOKEN(refreshToken: string) {
      this.refreshToken = refreshToken;
    },
    logIn(data: object): Promise<any> {
      return authRequest.login(data)
        .then(res => {
          if (res.code === 200) {
            this.SET_ACCESS_TOKEN(res.data.accessToken);
            this.SET_ACCESS_TOKEN(res.data.refreshToken);
            authCache.setToken(res.data);
          }
          return res;
        });
    },
    logOut() {
      return authRequest.logout().then(res => {
        if (res.code === 200) {
          message("登出成功", { type: "success" });
        };
        return res;
      }).finally(() => {
        authCache.removeToken();
        useMultiTagsStoreHook().handleTags("equal", [...routerArrays]);
        resetRouter();
        router.push("/login");
      })
    },
    refreshToken(): Promise<any> {
      return authRequest.refreshToken({ refreshToken: this.refreshToken }).then(res => {
        if (res.code === 200) {
          this.SET_ACCESS_TOKEN(res.data.accessToken);
          this.SET_REFHRESH_TOKEN(res.data.accessToken);
          authCache.setToken(res.data);
        }
        return res;
      })
    },
    userInfo(): Promise<any> {
      return new Promise<Result<authRequest.UserInfo>>((reslove, reject) => {
        authRequest.getUserInfo().then(res => {
          if (res.code === 200) {
            storageLocal().setItem(authCache.userKey, res.data);
            reslove(res);
          } else {
            reject(res);
          }
        })
      })
    }
  }
});

export function useAuthStoreHook() {
  return authStore(store);
}