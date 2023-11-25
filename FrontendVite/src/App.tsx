import "./App.css";
import { Route, Switch } from "react-router-dom";
import Register from "./components/RegisterForm";
import { Toaster } from "sonner";
import Login from "./components/LoginForm";
import Navbar from "./components/Navbar";
import FindName from "./components/pages/FindName";
import SignIn from "./components/pages/SignIn";
import Profile from "./components/pages/Profile";

function App() {
  return (
    <div className="flex flex-col h-screen w-screen justify-center items-center">
      <Toaster position="bottom-center" />
      <Navbar />
      <Switch>
        <Route path="/register">
          <Register />
        </Route>
        <Route path="/login">
          <Login />
        </Route>
        <Route path="/find-navn">
          <FindName />
        </Route>
        <Route path="/sign-in">
          <SignIn />
        </Route>
        <Route path="/profile">
          <Profile />
        </Route>
      </Switch>
    </div>
  );
}

export default App;
