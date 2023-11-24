import React, { useState, Fragment, useEffect } from "react";
import { useDispatch } from "react-redux";
import Friend from "../Friend/Friend";
import { setCurrentChat } from "../../../../redux/actions/chatActions";
import Modal from "../Modal/Modal";
import { chatService, accountService } from "../../../../_services";
import "./FriendList.css";
import { useSelector } from "react-redux";
import { AiOutlineUserAdd } from "react-icons/ai";

const FriendList = () => {
  const dispatch = useDispatch();
  const chats = useSelector((state) => state.chat.chats);
  const socket = useSelector((state) => state.chat.socket);

  const [showFriendsModal, setShowFriendsModal] = useState(false);
  const [suggestions, setSuggestions] = useState([]);

  useEffect(() => {
    (async () => {
      await accountService.loadStaff().then((res) => {
        setSuggestions(res);
      });
    })();
  }, []);

  const openChat = (chat) => {
    dispatch(setCurrentChat(chat));
  };

  const searchFriends = async (e) => {
    if (e.target.value.length > 0) {
      await accountService.searchStaff(e.target.value).then((res) => {
        setSuggestions(res);
      });
    }
    else{
      await accountService.loadStaff().then((res) => {
        setSuggestions(res);
      });
    }
  };

    const addNewFriend = async (id) => {
      await chatService.createChat(id).then((res) => {
      socket.invoke("AddFriend", { chats: res });
      setShowFriendsModal(false);
    });
  };

  return (
    <div id="friends" className="shadow-light">
      <div style={{paddingBottom:'10px',borderBottom: "1px solid black"}}>
        <div id="title" style={{marginTop:'10px', display:'flex', justifyContent: 'space-between', alignItems: 'center'}}>
          <h3 className="text-xl font-bold" style={{marginTop:'10px', display:'flex', justifyContent: 'center', alignItems: 'center'}}>Staff</h3>
          <button onClick={() => setShowFriendsModal(true)} style={{textAlign: 'center'}}>
            <AiOutlineUserAdd /><span style={{textAlign: 'center'}}>Staff</span>
          </button>
        </div>
      </div>

      <div id="friends-box">
        {chats.length > 0 ? (
          chats.map((chat, i) => {
            return (
              <>
                <Friend click={() => openChat(chat)} chat={chat} key={i} />
              </>
            );
          })
        ) : (
          <p id="no-chat">No Staff added</p>
        )}
      </div>
      {showFriendsModal && (
        <Modal click={() => setShowFriendsModal(false)}>
          <Fragment key="header">
            <h3 className="m-0">Create new chat</h3>
          </Fragment>

          <Fragment key="body">
            <p>Find Staff by typing their name bellow</p>
            <input
              style={{
                padding: '10px',
                fontSize: '16px',
                borderRadius: '10px',
                boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
                outline: 'none',
              }}
              onInput={(e) => searchFriends(e)}
              type="text"
              placeholder="Search..."
            />

            <div id="suggestions">
              {suggestions.map((user, i) => {
                return (
                  <div key={i} className="suggestion">
                    <p className="m-0">
                      {user.firstName} {user.lastName}
                    </p>
                    <button onClick={() => addNewFriend(user.id)}>
                      Choose
                    </button>
                  </div>
                );
              })}
            </div>
          </Fragment>
        </Modal>
      )}
    </div>
  );
};

export default FriendList;
