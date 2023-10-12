import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/checkIn`;

export const classService = {
    checkIn,
    getInfo,
};

function getInfo() {
    return fetchWrapper.get(`${baseUrl}`);
}

function checkIn(fromData) {
    return fetchWrapper.post(`${baseUrl}`, fromData);
}