import { ChatStatus } from "../../../_helpers";

export const userStatus = (user) => {
    return user.status === ChatStatus.Online ? ChatStatus.Online : ChatStatus.Offline
}