import * as http from '../base';
export interface Resource {
  /** 菜单类型（0代表菜单、1代表iframe、2代表外链、3代表按钮）*/
  type: number;
  parentId: number;
  title: string;
  name: string;
  path: string;
  component: string;
  permissionCode: string;
  rank: number;
  icon: string;
  frameSrc: string;
  frameLoading: boolean;
  keepAlive: boolean;
  showLink: boolean;
  showParent: boolean;
}
export function getResources(): Promise<http.Result<Resource[]>> {
  return http.get('/api/resource/list');
}