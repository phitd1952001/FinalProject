import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/dashboard`;

export const dashboardService = {
  get
};

function get() {
  return fetchWrapper.get(baseUrl);
}