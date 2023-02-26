import axios from "axios";
import { getUserData, updateUserAccessToken } from "../storage/auth.storage";

const instance = axios.create({
  // baseURL: "https://goldfish-app-ibq9e.ondigitalocean.app/api",
  baseURL: "https://localhost:7067/api",
  headers: {
    "Content-Type": "application/json",
  },
});

instance.interceptors.request.use(
  (config) => {
    const token = getUserData()?.accessToken;
    if (token) {
      if (config.headers) {
        config.headers["Authorization"] = "Bearer " + token;
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

instance.interceptors.response.use(
  (res) => {
    return res;
  },
  async (err) => {
    const originalConfig = err.config;

    if (
      !(originalConfig.url === "/tokens" && originalConfig.method === "put") &&
      err.response
    ) {
      if (err.response.status === 401 && !originalConfig._retry) {
        originalConfig._retry = true;

        try {
          const rs = await instance.put("/tokens", {
            token: getUserData()?.refreshToken,
          });

          const { accessToken } = rs.data;
          updateUserAccessToken(accessToken);

          return instance(originalConfig);
        } catch (_error) {
          return Promise.reject(_error);
        }
      }
    }

    return Promise.reject(err);
  }
);

export default instance;
