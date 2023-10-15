import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/checkIn`;

export const checkInService = {
    checkIn,
    getInfo,
};

function getInfo(qrCode) {
    return fetchWrapper.get(`${baseUrl}/${qrCode}`);
}

function checkIn(fromData) {
    return fetchWrapper.post(`${baseUrl}`, fromData);
}