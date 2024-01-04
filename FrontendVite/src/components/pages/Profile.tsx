import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthProvider";
import { User } from "../../types/types";

export default function Profile() {
  const { isLoggedIn } = useAuth();
  const [user, setUser] = useState<User | null>(null); // State to store user data

  // Fetch user data from backend
  useEffect(() => {
    const getUserInfo = async () => {
      try {
        const response = await fetch("http://localhost:5000/api/user", {
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

  // Function to remove partner
  const handleRemovePartner = async () => {
    const partnerEmail = user!.partner!.email;
    const updatedFormData = { email: partnerEmail || "" };

    try {
      const response = await fetch("http://localhost:5000/api/partner", {
        method: "DELETE",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedFormData),
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

  return (
    <div className="flex flex-col w-screen border text-start items-center h-screen pt-40">
      {user && (
        <div>
          <div className="flex flex-col w-full items-center justify-center gap-4">
            <h1>Din profil</h1>
            <div className="flex flex-col items-center gap-1">
              <p>Velkommen tilbage!</p>
              {user.partner ? (
                <div className=" flex flex-col gap-2  items-center">
                  <p className="">Din partner:</p>
                  <div className="flex justify-between gap-2">
                    <span className="border-button ">
                      {user.partner.firstName} - {user.partner.email}
                    </span>
                    <button
                      onClick={handleRemovePartner}
                      className="border-button border-none bg-rose-500 text-white font-normal"
                    >
                      Fjern partner
                    </button>
                  </div>
                </div>
              ) : (
                <p>Du har ikke en partner tilknyttet</p>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
