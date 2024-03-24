import * as http from "./base";

type Result = {
  success: boolean;
  data: Array<any>;
};

export const getAsyncRoutes = () => {
  // return http.request<Result>("get", "/get-async-routes");
  return http.get("/api/auth/routes");
};
