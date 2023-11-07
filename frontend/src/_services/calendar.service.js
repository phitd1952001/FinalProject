import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/calendar`;

export const calendarService = {
  getAll,
  getByUserId
};

function getAll() {
  return fetchWrapper.get(baseUrl);
}

function getByUserId() {
  return fetchWrapper.get(`${baseUrl}/user`);
}
