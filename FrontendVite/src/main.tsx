import { BrowserRouter } from "react-router-dom";
import React from "react";

import App from "./App.tsx";
import "./index.css";
import ReactDOM from "react-dom";
import { AuthProvider } from "./context/AuthProvider.tsx";

ReactDOM.render(
  <React.StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <App />
      </AuthProvider>
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById("root")
);
