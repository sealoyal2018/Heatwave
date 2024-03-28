
import * as http from '../base';

export type AppPlatform = {
  Id?: number;
  IsDeleted: boolean;
  Name: string;
  Key: string;
  Secret: string;
}

/**
 * 分页获取应用程序数据
 * @param data 分页获取应用程序数据
 * @returns 
 */
export function getAppPlatforms(data: Object): Promise<http.Result<AppPlatform>> {
  return http.post('/api/appplatform/page', data);
}

