import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthProvider";
import { MatchedBabyNames } from "../../types/types";
import MaleIcon from "../MaleIcon";
import FemaleIcon from "../FemaleIcon";

export default function Matches() {
  const { isLoggedIn } = useAuth();
  const [matches, setMatches] = useState<MatchedBabyNames>();
  const [activeFilter, setActiveFilter] = useState("all");

  // Fetch matches from backend
  useEffect(() => {
    const getMatches = async () => {
      try {
        const response = await fetch("http://localhost:5000/api/babynames/matches", {
          method: "GET",
          credentials: "include",
        });
        if (response.ok) {
          const matches = await response.json();
          setMatches(matches);
        } else {
          throw new Error("User not found");
        }
      } catch (error) {
        console.error(error);
      }
    };

    if (isLoggedIn) {
      getMatches();
    }
  }, [isLoggedIn]);

  if (!isLoggedIn) {
    return (
      <div>
        <p>Du skal være logget ind for at se denne side</p>
      </div>
    );
  }

  const handleFilters = (filter: string) => {
    setActiveFilter(filter);
  };

  const filterNames = (name: any) => {
    switch (activeFilter) {
      case "male":
        return name.isMale;
      case "female":
        return name.isFemale;
      case "unisex":
        return name.isMale && name.isFemale;
      default:
        return true;
    }
  };

  return (
    <div className="flex flex-col w-screen text-start items-center h-screen pt-40">
      <div className="flex flex-col w-full items-center justify-center">
        <h1 className="pb-5">Dine Matches</h1>
        <div className="flex pb-5">
          <p className="text-lg">Dette er en liste af dig og din partners matchede navne:</p>
        </div>
        <div className="flex gap-2 pb-5">
          <button
            className={`border-button ${activeFilter === "all" ? "bg-orange-200" : ""}`}
            onClick={() => handleFilters("all")}
          >
            Se alle matchede navne
          </button>
          <button
            className={`border-button ${activeFilter === "female" ? "bg-orange-200" : ""}`}
            onClick={() => handleFilters("female")}
          >
            Se pige navne
          </button>
          <button
            className={`border-button ${activeFilter === "male" ? "bg-orange-200" : ""}`}
            onClick={() => handleFilters("male")}
          >
            Se drenge navne
          </button>
          <button
            className={`border-button ${activeFilter === "unisex" ? "bg-orange-200" : ""}`}
            onClick={() => handleFilters("unisex")}
          >
            Se unisex navne
          </button>
        </div>
        <div className="flex flex-col w-full items-center">
          <div className="flex flex-col ">
          {matches &&
            matches.likedBabyNames &&
            matches.likedBabyNames.filter(filterNames).map((name) => (
              <div key={name.id} className="flex gap-2 ">
                {name.isMale && <MaleIcon />}
                {name.isFemale && <FemaleIcon />}
                <p>{name.name}</p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
