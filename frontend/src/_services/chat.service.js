import { fetchWrapper } from "../_helpers";
import config from "config";

const apiUrl = config.apiUrl;
const baseUrl = `${apiUrl}/chats`;

export const chatService = {
    fetchChats,
    uploadImageChats,
    paginateMessages,
    createChat,
    addFriendToGroupChat,
    leaveCurrentChat,
    deleteCurrentChat
};

const fetchChats= () =>
    fetchWrapper.get(baseUrl);

const uploadImageChats= (data) =>
    fetchWrapper.post(`${baseUrl}/upload-image`, data);

const paginateMessages = (id, page) =>
    fetchWrapper.get(`${baseUrl}/messages/${id}/${page}`);

const createChat = (partnerId) =>
    fetchWrapper.post(`${baseUrl}/create`,{ partnerId });

const addFriendToGroupChat = (userId, chatId) =>
    fetchWrapper.post(`${baseUrl}/add-user-to-group`, { userId, chatId });

const leaveCurrentChat = (chatId) =>
    fetchWrapper.post(`${baseUrl}/leave-current-chat`, { chatId });

const deleteCurrentChat = (chatId) =>
    fetchWrapper.delete(`${baseUrl}/${chatId}`);