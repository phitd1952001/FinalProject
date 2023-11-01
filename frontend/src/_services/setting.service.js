import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/setting`;

export const settingsService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  generateSchedule
};

function getAll() {
  return fetchWrapper.get(baseUrl);
}

function generateSchedule() {
  return fetchWrapper.get(`${apiUrl}/Schedule`);
}

function getById(id) {
  return fetchWrapper.get(`${baseUrl}/${id}`);
}

function create(params) {
  return fetchWrapper.post(baseUrl, params);
}

function update(id, params) {
  return fetchWrapper.put(`${baseUrl}/${id}`, params);
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`);
}