import { onMounted, ref } from "vue";
import { getCaptcha } from "@/api/auth";
import { delay } from "@pureadmin/utils";

export const useImageVerify = imgCode => {
  // const imgCode = ref("");
  const imgUrl = ref("");
  const loading = ref(false);

  function getImgCode() {
    loading.value = true;
    getCaptcha()
      .then(res => {
        if (res.code === 200) {
          imgUrl.value = `data:image/jpeg;base64,${res.data.captcha}`;
          imgCode.value = res.data.key;
        }
      })
      .finally(() => {
        delay(100).then(() => {
          loading.value = false;
        });
      });
  }

  onMounted(() => {
    getImgCode();
  });

  return {
    imgUrl,
    loading,
    imgCode,
    getImgCode
  };
};
