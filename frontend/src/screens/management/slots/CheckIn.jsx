import React, {useState, useEffect} from 'react'
import images from "../../../_asset/images";
import { QrReader } from 'react-qr-reader';
import { checkInService, alertService } from "../../../_services";

const CheckIn = ({id}) => {
    const [qrCode, setQrCode] = useState(null);
    const [user, setUser] = useState({});
    const [noAvatarImage, setNoAvatarImage] = useState(null);
    const [note, setNote] = useState("");
    const [noteError, setNoteError] = useState("");
    useEffect(() => {
        images.noAvatar.then((img) => setNoAvatarImage(img));
    }, []);

    useEffect(() => {
      checkInService.getInfo(encodeURIComponent(qrCode))
      .then((res)=>{
        console.log(res);
        setUser(res);
        alertService.success("Load Info successfully", {
          keepAfterRouteChange: true,
        });
      })
      .catch((error) => {
        alertService.error(error);
      });
    }, [qrCode]);

    const confirm=(isAccept, note)=>{
      let formData = {
        isAccept: isAccept,
        note: note,
        qrCodeString: qrCode,
        slotId: id
      }

      checkInService.checkIn(formData)
      .then((res)=>{
        setUser({});
        setQrCode(null);
        setNote("");
        setNoteError("");
        alertService.success("Check In successfully", {
          keepAfterRouteChange: true,
        });
      })
      .catch((error) => {
        alertService.error(error);
      });
    }

  const onCancel = () =>{
    if(note.length > 0){
      confirm(false, note)
    }
    else{
      setNoteError("Note is required")
    }
  }

  return (
    <>
    <div className="container" style={styles.body}>
      <div className="main-body" style={styles.mainBody}>
        <div className="row gutters-sm" style={styles.guttersSm}>
          <div className="col-md-8" style={styles.col}>
          <div className='overflow-hidden'>
            <QrReader
                    onResult={( result , error ) => {
                        if (!!result) {
                            setQrCode(result?.text);
                        }

                        if (!!error) {
                            console.info(error);
                        }
                    }}
                    style={{ width: '100%', height: '100px' }}
                />
          </div>
          </div>
          <div className="col-md-4" style={styles.col}>
            <div className="h-100 w-100 d-flex flex-column justify-content-center align-items-start">
              <div className="card w-100 mt-0" style={styles.card}>
                <div className="card-body" style={styles.cardBody}>
                  <div className="d-flex flex-column align-items-center text-center">
                    {user && user.avatar ? (
                      <img
                        src={user.avatar}
                        alt="Admin"
                        className="rounded-circle"
                        width="120"
                        height="120"
                      />
                    ) : (
                      <img
                        src={noAvatarImage}
                        alt=""
                        className="rounded-circle"
                        width="120"
                        height="120"
                      />
                    )}

                    <div className="mt-3">
                      <h4>
                        {user.firstName} {user.lastName}
                      </h4>
                      <p className="text-secondary mb-1">
                        {user.managementCode}
                      </p>
                      <p className="text-muted font-size-sm">
                        {user.dateOfBirth && new Date(user.dateOfBirth).toISOString().substr(0, 10)}
                      </p>
                      <textarea value={note} onChange={(e)=>setNote(e.target.value)} placeholder='Note' className="text-muted"/>
                      {noteError.length > 0 && (<span className='text-danger mb-1'>{noteError}</span>)}
                      <div className="d-flex justify-content-center">
                        <button onClick={()=>onCancel()} className="btn btn-danger">Cancel</button>
                        <button onClick={()=>confirm(true,"This is Accept")} className="btn btn-primary ml-2">Accept</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            
            </div>
          </div>
        </div>
      </div>
    </div>
  </>
  )
}

const styles = {
    body: {
      color: "#1a202c",
      textAlign: "left",
      backgroundColor: "#e2e8f0",
      boxShadow:
        "rgba(0, 0, 0.3) 0px 6px 8px -4px, rgba(0, 0.5, 0.4, 0.06) 0px 2px 4px -1px",
    },
    mainBody: {
      padding: "15px",
    },
    card: {
      boxShadow: "0 4px 6px -1px rgba(0,0,0,.1), 0 2px 4px -1px rgba(0,0,0,.06)",
      position: "relative",
      display: "flex",
      flexDirection: "column",
      minWidth: "0",
      wordWrap: "break-word",
      backgroundColor: "#fff",
      backgroundClip: "border-box",
      border: "0 solid rgba(0,0,0,.125)",
      borderRadius: ".25rem",
    },
    cardBody: {
      flex: "1 1 auto",
      minHeight: "1px",
      padding: "1rem",
    },
    guttersSm: {
      marginRight: "-8px",
      marginLeft: "-8px",
    },
    col: {
      paddingRight: "8px",
      paddingLeft: "8px",
    },
    mb3: {
      marginBottom: "1rem",
    },
    bgGray300: {
      backgroundColor: "#e2e8f0",
    },
    h100: {
      height: "100%",
    },
    shadowNone: {
      boxShadow: "none",
    },
  };

export default CheckIn