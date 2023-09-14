import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/subjects`;

export const subjectService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  getFields,
  uploadExcels,
  finalUploadExcels
};

function getAll() {
  return fetchWrapper.get(baseUrl);
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

//excel
function getFields() {
  return fetchWrapper.get(`${baseUrl}/available-fields`);
}

function uploadExcels(fromData) {
  return fetchWrapper.postFormData(`${baseUrl}/upload-excel`, fromData);
}

function finalUploadExcels(fromData) {
  return fetchWrapper.postFormData(`${baseUrl}/final-upload-excel`, fromData);
}
