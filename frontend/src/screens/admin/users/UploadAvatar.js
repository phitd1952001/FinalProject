import React, { useState, useEffect } from "react";
import { accountService, alertService } from "../../../_services";
import images from "../../../_asset/images";

const styles = {
  avatarContainer: {
    position: "relative",
    width: "150px",
    height: "150px",
    borderRadius: "50%",
    overflow: "hidden",
  },
  avatar: {
    width: "100%",
    height: "100%",
    objectFit: "cover",
  },
  changeAvatar: {
    position: "absolute",
    bottom: "10px",
    left: "50%",
    transform: "translateX(-50%)",
    backgroundColor: "#007bff",
    color: "#fff",
    border: "none",
    padding: "5px 10px",
    borderRadius: "5px",
    cursor: "pointer",
  },
  deleteIcon: {
    position: "absolute",
    top: "10px",
    right: "10px",
    color: "#dc3545",
    cursor: "pointer",
    zIndex: 1,
  },
};

function UploadAvatar({ user, onHide }) {
    function upload() {
    accountService
      .handleUpload(user.id, event.target.files[0])
      .then(() => {
        alertService.success("Upload successful", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        console.log(error)
        onHide();
        alertService.error(error);
      });
  }

  const [noAvatarImage, setNoAvatarImage] = useState(null);

  useEffect(() => {
    images.noAvatar.then((img) => setNoAvatarImage(img));
  }, []);

  return (
    <div
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100%",
      }}
    >
      <div>
        <div className="container my-5">
          <div style={styles.avatarContainer}>
            {user !== undefined && user.avatar ? (
              <img style={styles.avatar} src={user.avatar} alt="Avatar" />
            ) : (
              <img style={styles.avatar} src={noAvatarImage} alt="Avatar" />
            )}
          </div>
        </div>
        <input type="file" onChange={upload} />
      </div>
    </div>
  );
}

export default UploadAvatar;
