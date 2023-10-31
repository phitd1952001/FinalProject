import { BehaviorSubject } from "rxjs";

import { fetchWrapper, history } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const userSubject = new BehaviorSubject(null);
const baseUrl = `${apiUrl}/users`;

export const accountService = {
  login,
  logout,
  refreshToken,
  register,
  verifyEmail,
  forgotPassword,
  validateResetToken,
  resetPassword,
  getAll,
  getById,
  create,
  update,
  updateSelf,
  handleUpload,
  delete: _delete,
  getFields,
  uploadExcels,
  finalUploadExcels,
  user: userSubject.asObservable(),
  get userValue() {
    return userSubject.value;
  },
};

function login(email, password) {
  return fetchWrapper
    .post(`${baseUrl}/authenticate`, { email, password })
    .then((user) => {
      // publish user to subscribers and start timer to refresh token
      userSubject.next(user);
      startRefreshTokenTimer();
      return user;
    });
}

function logout() {
  // revoke token, stop refresh timer, publish null to user subscribers and redirect to login page
  fetchWrapper.post(`${baseUrl}/revoke-token`, {});
  stopRefreshTokenTimer();
  userSubject.next(null);
  history.push("/account/login");
}

function refreshToken() {
  return fetchWrapper.post(`${baseUrl}/refresh-token`, {}).then((user) => {
    // publish user to subscribers and start timer to refresh token
    userSubject.next(user);
    startRefreshTokenTimer();
    return user;
  });
}

function register(params) {
  return fetchWrapper.post(`${baseUrl}/register`, params);
}

function verifyEmail(token) {
  return fetchWrapper.post(`${baseUrl}/verify-email`, { token });
}

function forgotPassword(email) {
  return fetchWrapper.post(`${baseUrl}/forgot-password`, { email });
}

function validateResetToken(token) {
  return fetchWrapper.post(`${baseUrl}/validate-reset-token`, { token });
}

function resetPassword({ token, password, confirmPassword }) {
  return fetchWrapper.post(`${baseUrl}/reset-password`, {
    token,
    password,
    confirmPassword,
  });
}

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
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((user) => {
    // update stored user if the logged in user updated their own record
    if (user.id === userSubject.value.id) {
      // publish updated user to subscribers
      user = { ...userSubject.value, ...user };
      userSubject.next(user);
    }
    return user;
  });
}

function updateSelf(id, params) {
  return fetchWrapper.put(`${baseUrl}/update-self/${id}`, params).then((user) => {
    // update stored user if the logged in user updated their own record
    if (user.id === userSubject.value.id) {
      // publish updated user to subscribers
      user = { ...userSubject.value, ...user };
      userSubject.next(user);
    }
    return user;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`).then((x) => {
    // auto logout if the logged in user deleted their own record
    if (id === userSubject.value.id) {
      logout();
    }
    return x;
  });
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

// helper functions

let refreshTokenTimeout;

function startRefreshTokenTimer() {
  // parse json object from base64 encoded jwt token
  const jwtToken = JSON.parse(atob(userSubject.value.jwtToken.split(".")[1]));

  // set a timeout to refresh the token a minute before it expires
  const expires = new Date(jwtToken.exp * 1000);
  const timeout = expires.getTime() - Date.now() - 60 * 1000;
  refreshTokenTimeout = setTimeout(refreshToken, timeout);
}

function stopRefreshTokenTimer() {
  clearTimeout(refreshTokenTimeout);
}

function handleUpload(id, file) {
  const formData = new FormData();
  formData.append("file", file);

  return fetchWrapper
    .postUpload(`${baseUrl}/avatar/upload/${id}`, formData);
}
