import { http } from "@/utils/http";

const { VITE_HOST_API_URL } = import.meta.env;

export interface Result<T> {
  msg: string;
  code: number;
  data: T
}

export type EntityBase = {
  id: number;
  createdBy: number;
  createdTime: any;
  modifiedBy: number;
  modifiedTime: any;
  isDeleted: boolean;
  deletedTime: any;
  deletedBy: number;
}


/**
 *
 * post method
 *
 * @param url 请求地址
 * @param data 请求参数
 * @returns Promise<Result<P>>
 */
export function post<Q, T>(url: string, data?: Q): Promise<Result<T>> {
  return http.post<Q, Result<T>>(`${VITE_HOST_API_URL}${url}`, { data: data });
}

/**
 * get method
 * @param url 请求地址
 * @param params 请求参数
 * @returns Promise<Result<P>>
 */
export function get<Q, T>(url: string, params?: Q): Promise<Result<T>> {
  return http.get<Q, Result<T>>(`${VITE_HOST_API_URL}${url}`, { params });
}

/**
 * put
 * @param url path
 * @param params query
 * @param data body
 * @returns .
 */
export function put<Q, T>(
  url: string,
  params?: Q,
  data?: T
): Promise<Result<String>> {
  return http.request("put", `${VITE_HOST_API_URL}${url}`, { params: params, data: data });
}
/**
 * delete
 * @param url .
 * @param params .
 * @param data .
 * @returns .
 */
export function deleteRequest<Q, T>(
  url: string,
  params?: Q,
  data?: T
): Promise<Result<String>> {
  return http.request("delete", `${VITE_HOST_API_URL}${url}`, { params: params, data: data });
}
