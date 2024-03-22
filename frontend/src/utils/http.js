import service from './request';
import { ElNotification, ElMessageBox, ElMessage, ElLoading } from 'element-plus'
let downloadLoadingInstance = null;

export async function post(url, body, isToken = false) {
    downloadLoadingInstance = ElLoading.service({ text: "正在下载数据，请稍候", background: "rgba(0, 0, 0, 0.7)", });
    console.log('body>>>>>>',body);
    const response = await service({
        url: url,
        method: 'post',
        data: body,
        headers: { 'Content-Type': 'application/json', 'isToken': isToken },
        responseType: 'json'
    });
    downloadLoadingInstance.close();
    if (response.status === 200) {
        const {data, code} = response.data;
        if (code === 200) {
            return data;
        }
        return null;
    }
    if (response.status === 404)
        return null;
    //   url: url,
    //   method: 'get',
    //   params: query,
    //   headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    //   responseType: 'blob'
    // }).then(async (data) => {
    //   downloadLoadingInstance.close();
    // }).catch((r) => {
    //   downloadLoadingInstance.close();
    // })
}

export async function get(url, query, isToken = false) {
    downloadLoadingInstance = ElLoading.service({ text: "正在下载数据，请稍候", background: "rgba(0, 0, 0, 0.7)", });

    const response = await service({
        url: url,
        method: 'get',
        params: query,
        headers: { 'isToken': isToken },
        responseType: 'json'
    });
    downloadLoadingInstance.close();
    if (response.status === 200) {
        const {data, code} = response.data;
        if (code === 200) {
            return data;
        }
        return null;
    }
    if (response.status === 404)
        return null;
}