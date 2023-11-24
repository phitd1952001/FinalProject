import React, { useState, useRef, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {faFaceSmile, faPhotoFilm, faTimes, faUpload} from '@fortawesome/free-solid-svg-icons';
import data from "@emoji-mart/data";
import Picker from "@emoji-mart/react";
import { incrementScroll } from "../../../../redux/actions/chatActions";
import "./MessageInput.css";
import { chatService, accountService } from "../../../../_services";

const MessageInput = ({ chat }) => {
  const dispatch = useDispatch();
  const user = accountService.userValue;
  const socket = useSelector((state) => state.chat.socket);
  const newMessage = useSelector((state) => state.chat.newMessage);

  const fileUpload = useRef();
  const msgInput = useRef();

  const [message, setMessage] = useState("");
  const [image, setImage] = useState("");
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const [showNewMessageNotification, setShowNewMessageNotification] =
    useState(false);

  const handleMessage = (e) => {
    const value = e.target.value;
    setMessage(value);

    const receiver = {
      chatId: chat.id,
      fromUser: user,
      toUserId: chat.users.map((user) => user.id),
    };

    if (value.length === 1) {
      receiver.typing = true;
      socket.invoke("Typing", receiver);
    }

    if (value.length === 0) {
      receiver.typing = false;
      socket.invoke("Typing", receiver);
    }

    // notify other users that this user is typing something
  };

  const handleKeyDown = (e, imageUpload) => {
    if (e.key === "Enter") sendMessage(imageUpload);
  };

  const sendMessage = (imageUpload) => {
    if (message.length < 1 && !imageUpload) return;

    const msg = {
      type: imageUpload ? "image" : "text",
      fromUser: user,
      toUserId: chat.users.map((user) => user.id),
      chatId: chat.id,
      message: imageUpload ? imageUpload.url : message,
    };

    setMessage("");
    setImage("");
    setShowEmojiPicker(false);

    // send message with socket
    socket.invoke("Message", msg);
  };

  const handleImageUpload = async () => {
    const formData = new FormData();
    formData.append("id", chat.id);
    formData.append("image", image);

    await chatService.uploadImageChats(formData).then((res)=>{
      sendMessage(res);
    });
  };

  const selectEmoji = (emoji) => {
    const startPosition = msgInput.current.selectionStart;
    const endPosition = msgInput.current.selectionEnd;
    const emojiLength = emoji.native.length;
    const value = msgInput.current.value;
    setMessage(
      value.substring(0, startPosition) +
        emoji.native +
        value.substring(endPosition, value.length)
    );
    msgInput.current.focus();
    msgInput.current.selectionEnd = endPosition + emojiLength;
  };

  useEffect(() => {
    const msgBox = document.getElementById("msg-box");
    if (
      !newMessage.seen &&
      newMessage.chatId === chat.id &&
      msgBox.scrollHeight !== msgBox.clientHeight
    ) {
      if (msgBox.scrollTop > msgBox.scrollHeight * 0.3) {
        dispatch(incrementScroll());
      } else {
        setShowNewMessageNotification(true);
      }
    } else {
      setShowNewMessageNotification(false);
    }
  }, [newMessage, dispatch]);

  const showNewMessage = () => {
    dispatch(incrementScroll());
    setShowNewMessageNotification(false);
  };

  return (
    <div id="input-container">
      <div id="image-upload-container">
        <div>
          {showNewMessageNotification ? (
            <div id="message-notification" onClick={showNewMessage}>
              <FontAwesomeIcon icon="bell" className="fa-icon" />
              <p className="m-0">new message</p>
            </div>
          ) : null}
        </div>

        <div id="image-upload">
          {image.name ? (
            <div id="image-details">
              <p className="m-0">{image.name}</p>
              <FontAwesomeIcon
                onClick={handleImageUpload}
                icon={faUpload}
                className="fa-icon"
              />
              <FontAwesomeIcon
                onClick={() => setImage("")}
                icon={faTimes}
                className="fa-icon"
              />
            </div>
          ) : null}
          <FontAwesomeIcon
              onClick={() => fileUpload.current.click()}
              icon={faPhotoFilm}
              className="fa-icon"
          />
        </div>
      </div>
      <div id="message-input">
        <input
          ref={msgInput}
          value={message}
          type="text"
          placeholder="Message..."
          onChange={(e) => handleMessage(e)}
          onKeyDown={(e) => handleKeyDown(e, false)}
        />
        <FontAwesomeIcon className="fa-icon" icon={faFaceSmile}  onClick={() => setShowEmojiPicker(!showEmojiPicker)} />
      </div>

      <input
        id="chat-image"
        ref={fileUpload}
        type="file"
        onChange={(e) => setImage(e.target.files[0])}
      />

      {showEmojiPicker ? (
        <div style={{ position: "absolute", bottom: "20px", right: "20px" }}>
          <Picker
            data={data}
            theme="light"
            previewEmoji="point_up"
            onEmojiSelect={selectEmoji}
          />
        </div>
      ) : null}
    </div>
  );
};

export default MessageInput;
