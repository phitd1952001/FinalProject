import { applyMiddleware, createStore, combineReducers, compose } from "redux";

import chat from "./reducers/chatReducer";

export const allReducers = combineReducers({
    chat
});

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
export const store = createStore(allReducers, composeEnhancers(applyMiddleware()));
