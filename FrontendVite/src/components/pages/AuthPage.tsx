import React, { useState } from "react";
import LoginForm from "../LoginForm";
import RegisterForm from "../RegisterForm";


const AuthPage: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true);

  const toggleForm = () => {
    setIsLogin((prev) => !prev);
  };

  return (
    <div className="flex flex-col gap-4 w-96 border border-orange-300 p-8 shadow-lg">
      {isLogin ? (
        <LoginForm />
      ) : (
        <RegisterForm />
      )}
      <div className="text-center">
        {isLogin ? (
          <p>
            Ikke medlem?{" "}
            <span className="text-orange-500 cursor-pointer underline" onClick={toggleForm}>
              Opret bruger
            </span>
          </p>
        ) : (
          <p>
            Allerede medlem?{" "}
            <span className="text-orange-500 cursor-pointer underline" onClick={toggleForm}>
              Log ind
            </span>
          </p>
        )}
      </div>
    </div>
  );
};

export default AuthPage;
