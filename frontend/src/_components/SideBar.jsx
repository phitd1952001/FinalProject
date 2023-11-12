import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { accountService } from "../_services";
import images from "../_asset/images";
import {
  AiOutlineUser,
  AiOutlineLogout,
  AiFillHome,
  AiFillDatabase,
  AiFillContainer,
  AiOutlineBank,
  AiOutlineBars,
  AiOutlineDown,
  AiTwotoneShop,
  AiFillBuild,
  AiFillDashboard,
  AiFillCalendar,
  AiFillSetting,
} from "react-icons/ai";
import { Role } from "../_helpers";

const styles = {
  avatar: {
    borderRadius: "50%",
    overflow: "hidden",
  },
};

const SideBar = () => {
  const [user, setUser] = useState({});
  const [noAvatarImage, setNoAvatarImage] = useState(null);
  const [managementToggle, setManagementToggle] = useState(false);
  const [scheduleToggle, setScheduleToggle] = useState(false);

  useEffect(() => {
    if (user) {
      images.noAvatar.then((img) => setNoAvatarImage(img));
    }
    const subscription = accountService.user.subscribe((x) => setUser(x));
    return () => subscription.unsubscribe();
  }, []);

  // only show nav when logged in
  if (!user) return null;

  const handleManagementToggle = (e) => {
    setScheduleToggle(false);
    setManagementToggle((prev) => !prev);
  };

  const handleScheduleToggle = (e) => {
    setManagementToggle(false);
    setScheduleToggle((prev) => !prev);
  };

  return (
    <aside className="w-100 h-100 bg-dark">
      <div className="w-100 px-3 py-4 overflow-auto rounded">
        <ul className="list-unstyled w-100">
          {/* Avatar Image */}
          <li className="w-100 h-fit d-flex flex-row flex-md-column align-items-center justify-content-center cursor-pointer">
            <div className="shrink-0 ml-2 mb-2">
              {user && user.avatar ? (
                <img
                  style={styles.avatar}
                  src={user.avatar}
                  alt="Avatar"
                  className="avatar-img"
                  width="70"
                  height="70"
                />
              ) : (
                <img
                  src={noAvatarImage}
                  alt="Avatar"
                  className="avatar-img"
                  width="70"
                  height="70"
                />
              )}
            </div>
            <div className="grow ml-3">
              <p className="d-none d-sm-inline-block text-sm font-weight-semibold text-white">
                {user && user.firstName ? <>Hi {user.firstName}!</> : <>Hi!</>}
              </p>
            </div>
          </li>

          {/* Home */}
          <li className="w-100">
            <Link
              to="/"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/" ? "bg-light text-dark" : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiFillHome
                  className={`${
                    location.pathname === "/" ? "text-dark" : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">Home</span>
              </div>
            </Link>
          </li>

          {/* Profile */}
          <li className="w-100">
            <Link
              to="/profile"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/profile"
                  ? "bg-light text-dark"
                  : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiOutlineUser
                  className={`${
                    location.pathname === "/profile"
                      ? "text-dark"
                      : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">Profile</span>
              </div>
            </Link>
          </li>

          {/* Dashboard */}
          <li className="w-100">
            <Link
              to="/dashboard"
              style={{ textDecoration: "none" }}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                location.pathname === "/dashboard"
                  ? "bg-light text-dark"
                  : "text-white"
              } rounded-lg sideBarBtn`}
            >
              <div className="d-flex align-items-center">
                <AiFillDashboard
                  className={`${
                    location.pathname === "/dashboard"
                      ? "text-dark"
                      : "text-white"
                  } w-5 h-5`}
                />
                <span className="d-none d-md-inline-block ml-3">DashBoard</span>
              </div>
            </Link>
          </li>

          {/* Manage Users */}
          {user.role === Role.Admin && (
            <li className="w-100">
              <Link
                to="/admin/users"
                style={{ textDecoration: "none" }}
                className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                  location.pathname === "/admin/users"
                    ? "bg-light text-dark"
                    : "text-white"
                } rounded-lg sideBarBtn`}
              >
                <div className="d-flex align-items-center">
                  <AiFillDatabase
                    className={`${
                      location.pathname === "/admin/users"
                        ? "text-dark"
                        : "text-white"
                    } w-5 h-5`}
                  />
                  <span className="d-none d-md-inline-block ml-3">
                    Manage Users
                  </span>
                </div>
              </Link>
            </li>
          )}

          {/* Managements */}
          <li className="w-100">
            <a
              style={{ textDecoration: "none" }}
              onClick={(e) => handleManagementToggle(e)}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                managementToggle ? "bg-light text-dark" : "text-white"
              } rounded-lg sideBarBtn mb-2`}
            >
              <div className="w-100 d-flex align-items-center justify-content-between">
                <div>
                  <AiOutlineBars
                    className={`${
                      managementToggle ? "text-dark" : "text-white"
                    } w-5 h-5`}
                  />
                  <span className="d-none d-md-inline-block ml-3">
                    Managements
                  </span>
                </div>
                <div className="d-flex align-items-center justify-content-end">
                  <AiOutlineDown
                    className={`${
                      managementToggle ? "text-dark" : "text-white"
                    } w-5 h-5`}
                  />
                </div>
              </div>
            </a>
            <ul
              className={`${
                managementToggle ? "d-flex flex-column d-block" : "d-none"
              }`}
            >
              {(user.role === Role.Admin || user.role == Role.Staff) && (
                <>
                  {/* Subjects */}
                  <li className="w-100">
                    <Link
                      to="/management/subjects"
                      style={{ textDecoration: "none" }}
                      className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                        location.pathname === "/management/subjects"
                          ? "bg-light text-dark"
                          : "text-white"
                      } rounded-lg sideBarBtn`}
                    >
                      <div className="d-flex align-items-center">
                        <AiFillContainer
                          className={`${
                            location.pathname === "/management/subjects"
                              ? "text-dark"
                              : "text-white"
                          } w-5 h-5`}
                        />
                        <span className="d-none d-md-inline-block ml-2">
                          Subjects
                        </span>
                      </div>
                    </Link>
                  </li>

                  {/* Rooms */}
                  <li className="w-100">
                    <Link
                      to="/management/rooms"
                      style={{ textDecoration: "none" }}
                      className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                        location.pathname === "/management/rooms"
                          ? "bg-light text-dark"
                          : "text-white"
                      } rounded-lg sideBarBtn`}
                    >
                      <div className="d-flex align-items-center">
                        <AiOutlineBank
                          className={`${
                            location.pathname === "/management/rooms"
                              ? "text-dark"
                              : "text-white"
                          } w-5 h-5`}
                        />
                        <span className="d-none d-md-inline-block ml-2">
                          Rooms
                        </span>
                      </div>
                    </Link>
                  </li>

                  {/* Classes */}
                  <li className="w-100">
                    <Link
                      to="/management/classes"
                      style={{ textDecoration: "none" }}
                      className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                        location.pathname === "/management/classes"
                          ? "bg-light text-dark"
                          : "text-white"
                      } rounded-lg sideBarBtn`}
                    >
                      <div className="d-flex align-items-center">
                        <AiTwotoneShop
                          className={`${
                            location.pathname === "/management/classes"
                              ? "text-dark"
                              : "text-white"
                          } w-5 h-5`}
                        />
                        <span className="d-none d-md-inline-block ml-2">
                          Classes
                        </span>
                      </div>
                    </Link>
                  </li>
                </>
              )}

              {/* Slots */}
              <li className="w-100">
                <Link
                  to="/management/slots"
                  style={{ textDecoration: "none" }}
                  className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                    location.pathname === "/management/slots"
                      ? "bg-light text-dark"
                      : "text-white"
                  } rounded-lg sideBarBtn`}
                >
                  <div className="d-flex align-items-center">
                    <AiFillBuild
                      className={`${
                        location.pathname === "/management/slots"
                          ? "text-dark"
                          : "text-white"
                      } w-5 h-5`}
                    />
                    <span className="d-none d-md-inline-block ml-2">Slots</span>
                  </div>
                </Link>
              </li>
            </ul>
          </li>

          {/* Schedule */}
          <li className="w-100">
            <a
              style={{ textDecoration: "none" }}
              onClick={(e) => handleScheduleToggle(e)}
              className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                scheduleToggle ? "bg-light text-dark" : "text-white"
              } rounded-lg sideBarBtn mb-2`}
            >
              <div className="w-100 d-flex align-items-center justify-content-between">
                <div>
                  <AiFillCalendar
                    className={`${
                      scheduleToggle ? "text-dark" : "text-white"
                    } w-5 h-5`}
                  />
                  <span className="d-none d-md-inline-block ml-3">
                    Schedule
                  </span>
                </div>
                <div className="d-flex align-items-center justify-content-end">
                  <AiOutlineDown
                    className={`${
                      scheduleToggle ? "text-dark" : "text-white"
                    } w-5 h-5`}
                  />
                </div>
              </div>
            </a>
            <ul
              className={`${
                scheduleToggle ? "d-flex flex-column d-block" : "d-none"
              }`}
            >
              {/* Calendar */}
              <li className="w-100">
                <Link
                  to="/schedule/calendar"
                  style={{ textDecoration: "none" }}
                  className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                    location.pathname === "/schedule/calendar"
                      ? "bg-light text-dark"
                      : "text-white"
                  } rounded-lg sideBarBtn`}
                >
                  <div className="d-flex align-items-center">
                    <AiFillCalendar
                      className={`${
                        location.pathname === "/schedule/calendar"
                          ? "text-dark"
                          : "text-white"
                      } w-5 h-5`}
                    />
                    <span className="d-none d-md-inline-block ml-2">
                      Calendar
                    </span>
                  </div>
                </Link>
              </li>
              {(user.role === Role.Admin || user.role == Role.Staff) && (
             
              <li className="w-100">
                <Link
                  to="/schedule/setting"
                  style={{ textDecoration: "none" }}
                  className={`d-flex align-items-center p-2 text-base justify-content-between  ${
                    location.pathname === "/schedule/setting"
                      ? "bg-light text-dark"
                      : "text-white"
                  } rounded-lg sideBarBtn`}
                >
                  <div className="d-flex align-items-center">
                    <AiFillSetting
                      className={`${
                        location.pathname === "/schedule/setting"
                          ? "text-dark"
                          : "text-white"
                      } w-5 h-5`}
                    />
                    <span className="d-none d-md-inline-block ml-2">
                      Setting
                    </span>
                  </div>
                </Link>
              </li>
              )}
            </ul>
          </li>

          <li className="w-100">
            <div
              onClick={accountService.logout}
              className=" sideBarBtn d-flex align-items-center p-2 text-base font-weight-normal rounded-lg text-white hover-bg-gray-100 dark:hover-bg-gray-700"
            >
              <AiOutlineLogout className="text-white w-5 h-5" />
              <span className="d-none d-md-inline-block ml-3">Logout</span>
            </div>
          </li>
        </ul>
      </div>
    </aside>
  );
};

export { SideBar };
