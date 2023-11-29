import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthProvider";

interface User {
  id: string;
  firstName: string;
  email: string;
  partner: User | null;
}

export default function Profile() {
  const { isLoggedIn } = useAuth();
  const [user, setUser] = useState<User | null>(null); // State to store user data

  useEffect(() => {
    const getUserInfo = async () => {
      try {
        const response = await fetch("http://localhost:5000/user", {
          method: "GET",
          credentials: "include",
        });
        if (response.ok) {
          const userData = await response.json();
          setUser(userData);
        } else {
          throw new Error("User not found");
        }
      } catch (error) {
        console.error(error);
      }
    };

    if (isLoggedIn) {
      getUserInfo();
    }
  }, [isLoggedIn]);

  if (!isLoggedIn) {
    return (
      <div>
        <p>Du skal være logget ind for at se denne side</p>
      </div>
    );
  }

  return (
    <div>
      {user && (
        <div className="flex flex-col w-screen text-start items-center h-screen pt-40">
          <h1>Din profil</h1>
          <p>Velkommen, {user.firstName}</p>
          <p>Email: {user.email}</p>
          {user.partner && (
            <div className="flex flex-col items-center">
              <p className="">Din partner: {user.partner.firstName}</p>
              <p className="">Din partners email: {user.partner.email}</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
