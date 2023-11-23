import { applyMiddleware, createStore, combineReducers, compose } from "redux";

import chatReducer from "./reducers/chatReducer";

export const allReducers = combineReducers({
    chatReducer
});

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
export const store = createStore(allReducers, composeEnhancers(applyMiddleware()));
