import React, { useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import FriendList from "./components/FriendList/FriendList";
import Messenger from "./components/Messenger/Messenger";
import "./Chat.css";
import { HubConnectionBuilder } from "@microsoft/signalr";
import {
  fetchChats,
  onlineFriends,
  onlineFriend,
  offlineFriend,
  setSocket,
  receivedMessage,
  senderTyping,
  createChat,
  addUserToGroup,
  leaveCurrentChat,
  deleteCurrentChat,
} from "../../redux/actions/chatActions";
import config from "config";
import { chatService, accountService } from "../../_services";

const Chat = () => {
  const signalRUrl = config.signalRUrl;
  const dispatch = useDispatch();
  const user = accountService.userValue;
  const chats = useSelector((state) => state.chat.chats);

  useEffect(() => {
    if (chats) {
      const connection = new HubConnectionBuilder()
        .withUrl(signalRUrl)
        .build();

      connection
        .start()
        .then(() => {
          dispatch(setSocket(connection));
          connection.invoke("Join", user);

          connection.on("typing", (sender) => {
            dispatch(senderTyping(sender));
          });

          connection.on("friends", (friends) => {
            dispatch(onlineFriends(friends));
          });

          connection.on("online", (onlineUser) => {
            dispatch(onlineFriend(onlineUser));
          });

          connection.on("offline", (offlineUser) => {
            dispatch(offlineFriend(offlineUser));
          });

          connection.on("received", (message) => {
            let payload = {
              message,
              userId: user.id,
            };
            dispatch(receivedMessage(payload));
          });

          connection.on("new-chat", (chat) => {
            dispatch(createChat(chat));
          });

          connection.on("added-user-to-group", (group) => {
            dispatch(addUserToGroup(group));
          });

          connection.on("remove-user-from-chat", (data) => {
            data.currentUserId = user.id;
            dispatch(leaveCurrentChat(data));
          });

          connection.on("delete-chat", (chatId) => {
            dispatch(deleteCurrentChat(chatId));
          });
        })
        .catch((err) => console.log(err));

      return () => {
        connection.stop();
      };
    }
  }, []);

  useEffect(() => {
    (async () => {
      const chatData = await getChats();
      dispatch(fetchChats(chatData));
    })();
  }, [dispatch, user]);

  const getChats = async () => {
    try {
      const res = await chatService.fetchChats();
      return res;
    } catch (error) {
      console.error(error);
      return [];
    }
  };

  return (
    <div id="chat-container" style={{marginTop:'50px'}}>
      <div id="chat-wrap">
        <FriendList />
        <Messenger />
      </div>
    </div>
  );
};

export default Chat;
