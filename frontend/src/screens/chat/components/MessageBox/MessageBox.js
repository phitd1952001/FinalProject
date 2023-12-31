import React, { useEffect, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import Message from "../Message/Message";
import { paginateMessages } from "../../../../redux/actions/chatActions";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import "./MessageBox.css";
import { chatService, accountService } from "../../../../_services";
import { useSelector } from "react-redux";
import {faSpinner} from "@fortawesome/free-solid-svg-icons";

const MessageBox = ({ chat }) => {
  const dispatch = useDispatch();

  const user = accountService.userValue;
  const scrollBottom = useSelector((state) => state.chat.scrollBottom);
  const senderTyping = useSelector((state) => state.chat.senderTyping);
  const [loading, setLoading] = useState(false);
  const [scrollUp, setScrollUp] = useState(0);

  const msgBox = useRef();

  const scrollManual = (value) => {
    msgBox.current.scrollTop = value;
  };

  const handleInfiniteScroll = async (e) => {
    if (e.target.scrollTop === 0) {
      setLoading(true);
      const pagination = chat.Pagination;
      const page = typeof pagination === "undefined" ? 1 : pagination.page;

      await chatService.paginateMessages(
        chat.id,
        parseInt(page) + 1
      ).then((res)=>{
        const { messages, pagination } = res;
        if (typeof messages !== "undefined" && messages.length > 0) {
          dispatch(paginateMessages(chat.id,messages,pagination));
          setScrollUp(scrollUp + 1);
        }

        setLoading(false);
      })
    }
  };

  useEffect(() => {
    setTimeout(() => {
      scrollManual(Math.ceil(msgBox.current.scrollHeight * 0.1));
    }, 100);
  }, [scrollUp]);

  useEffect(() => {
    if (
      senderTyping.typing &&
      msgBox.current.scrollTop > msgBox.current.scrollHeight * 0.3
    ) {
      setTimeout(() => {
        scrollManual(msgBox.current.scrollHeight);
      }, 100);
    }
  }, [senderTyping]);

  useEffect(() => {
    if (!senderTyping.typing) {
      setTimeout(() => {
        scrollManual(msgBox.current.scrollHeight);
      }, 100);
    }
  }, [scrollBottom]);

  return (
    <div onScroll={handleInfiniteScroll} id="msg-box" ref={msgBox}>
      {loading ? (
        <p className="loader m-0">
          <FontAwesomeIcon icon={faSpinner} className="fa-spin" />
        </p>
      ) : null}
      {chat.messages != null && chat.messages.map((message, index) => {
        return (
          <Message
            user={user}
            chat={chat}
            message={message}
            index={index}
            key={message.id}
          />
        );
      })}
      {senderTyping.typing && senderTyping.chatId === chat.id ? (
        <div className="message mt-5p">
          <div className="other-person">
            <p className="m-0">...</p>
          </div>
        </div>
      ) : null}
    </div>
  );
};

export default MessageBox;
