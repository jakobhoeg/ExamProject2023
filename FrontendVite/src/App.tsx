import "./App.css";
import { Route, Switch } from "react-router-dom";
import Register from "./components/RegisterForm";
import { Toaster } from "sonner";
import Login from "./components/LoginForm";
import Navbar from "./components/Navbar";
import SignIn from "./components/pages/SignIn";
import Profile from "./components/pages/Profile";
import Names from "./components/pages/Names";
import AddPartner from "./components/pages/AddPartner";
import Matches from "./components/pages/Matches";

function App() {
  return (
    <div className="flex flex-col w-screen justify-center items-center">
      <Toaster position="bottom-center" />
      <Navbar />
      <Switch>
        <Route path="/register">
          <Register />
        </Route>
        <Route path="/login">
          <Login />
        </Route>
        <Route path="/sign-in">
          <SignIn />
        </Route>
        <Route path="/profile">
          <Profile />
        </Route>
        <Route path="/navne">
          <Names />
        </Route>
        <Route path="/tilknyt-partner">
          <AddPartner />
        </Route>
        <Route path="/matches">
          <Matches />
        </Route>
      </Switch>
    </div>
  );
}

export default App;
