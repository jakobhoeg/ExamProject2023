import { useEffect, useState } from "react";
import "../../App.css";
import { HeartFilledIcon, HeartIcon } from "@radix-ui/react-icons";
import MaleIcon from "../MaleIcon";
import FemaleIcon from "../FemaleIcon";
import { BabyName, User } from "../../types/types";
import { useAuth } from "../../context/AuthProvider";

export default function Names() {
  const [babyNames, setBabyNames] = useState<BabyName[]>([]);
  const [index, setIndex] = useState(1);
  const [isMaleFilter, setIsMaleFilter] = useState(false);
  const [isFemaleFilter, setIsFemaleFilter] = useState(false);
  const [isInternationalFilter, setIsInternationalFilter] = useState(false);
  const [isSwipeMode, setSwipeMode] = useState(false);
  const [isListViewMode, setListViewMode] = useState(true);
  const [sortMethod, setSortMethod] = useState("name/asc");
  const [likedNames, setLikedNames] = useState<{ [key: string]: boolean }>({});
  const [user, setUser] = useState<User | null>(null);
  const { isLoggedIn } = useAuth();

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
    } else {
      // Update the UI if the user logs out
      setUser(null);
      setLikedNames({});
    }
  }, [isLoggedIn, likedNames]);

  const getBabyNames = async (index: number) => {
    try {
      const response = await fetch(
        `http://localhost:5000/babynames/?page=${index}`,
        {
          method: "GET",
        }
      );
      if (response.ok) {
        const babyNameData = await response.json();
        console.log(babyNameData);
        setBabyNames(babyNameData);
      } else {
        throw new Error("User not found");
      }
    } catch (error) {
      console.error(error);
    }
  };

  const getBabyFilter = async (index: number) => {
    try {
      const url = new URL(`http://localhost:5000/babynames/sort/${sortMethod}`);
      url.searchParams.append("page", index.toString());
      url.searchParams.append("isMale", isMaleFilter.toString());
      url.searchParams.append("isFemale", isFemaleFilter.toString());
      url.searchParams.append(
        "isInternational",
        isInternationalFilter.toString()
      );

      const response = await fetch(url.toString(), {
        method: "GET",
        credentials: "include",
      });

      if (response.ok) {
        const babyNameData = await response.json();
        console.log(babyNameData);
        setBabyNames(babyNameData);
      } else {
        throw new Error("User not found");
      }
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    const url = new URL(window.location.href);
    const pageIndex = url.searchParams.get("page");
    setIndex(Number(pageIndex) || 1);

    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyFilter(Number(pageIndex) || 1);
    } else {
      getBabyNames(Number(pageIndex) || 1);
    }
  }, [isMaleFilter, isFemaleFilter, isInternationalFilter, sortMethod]);

  const handleBackClick = () => {
    if (index > 1) {
      setIndex(index - 1);
      window.history.pushState({}, "", `/navne?page=${index - 1}`);
    } else {
      setIndex(1);
      window.history.pushState({}, "", `/navne?page=${1}`);
    }

    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyFilter(index - 1);
    } else {
      getBabyNames(index - 1);
    }
  };

  const handleNextClick = () => {
    setIndex(index + 1);
    window.history.pushState({}, "", `/navne?page=${index + 1}`);

    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyFilter(index + 1);
    } else {
      getBabyNames(index + 1);
    }
  };

  const handleFilterClick = (filter: string) => {
    switch (filter) {
      case "male":
        setIsMaleFilter(!isMaleFilter);
        if (isInternationalFilter && !isFemaleFilter) {
          setIsInternationalFilter(false);
        }
        break;
      case "female":
        setIsFemaleFilter(!isFemaleFilter);
        if (isInternationalFilter && !isMaleFilter) {
          setIsInternationalFilter(false);
        }
        break;
      case "international":
        if (!isMaleFilter && !isFemaleFilter) {
          setIsInternationalFilter(!isInternationalFilter);
          setIsMaleFilter(true);
        } else {
          setIsInternationalFilter(!isInternationalFilter);
        }
        break;
      default:
        break;
    }
  };

  const handleViewClick = (view: string) => {
    switch (view) {
      case "list":
        setListViewMode(true);
        setSwipeMode(false);
        break;
      case "swipe":
        setSwipeMode(true);
        setListViewMode(false);
        break;
      default:
        break;
    }
  };

  const checkFiltersAndFetchNames = () => {
    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyFilter(index);
    } else {
      getBabyNames(index);
    }
  };

  const handleLikeClick = async (babyName: BabyName) => {
    try {
      // Update local state to toggle UI immediately
      setLikedNames((prevLikedNames) => ({
        ...prevLikedNames,
        [babyName.id]: !prevLikedNames[babyName.id],
      }));
  
      // Update DB
      const response = await fetch(`http://localhost:5000/babynames/like`, {
        method: "PUT",
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(babyName),
        credentials: "include",
      });
  
      if (response.ok) {
        // Update UI by fetching the latest list of baby names (with updated likes)
        checkFiltersAndFetchNames();
      }
    } catch (error) {
      console.error(error);
    }
  };
  

  return (
    <div className="flex flex-col w-screen pt-40 pb-20 justify-center items-center">
      <div className="flex flex-col justify-center items-center gap-8">
        <h1 className="text-5xl">Alle navne</h1>
        <div className="flex items-center justify-center w-full">
          <button
            className={`border-button ${isListViewMode ? "bg-orange-200" : ""}`}
            onClick={() => handleViewClick("list")}
          >
            Liste Mode
          </button>
          <button
            className={`border-button ${isSwipeMode ? "bg-orange-200" : ""}`}
            onClick={() => handleViewClick("swipe")}
          >
            Swipe Mode
          </button>
        </div>

        <div className="flex flex-col items-center justify-center w-[550px] gap-4">
          <div className="flex justify-center">
            <h2 className="text-2xl">Opsæt filter</h2>
          </div>
          <div className="flex justify-center gap-4">
            <button
              className={`border-button ${isMaleFilter ? "bg-orange-200" : ""}`}
              onClick={() => handleFilterClick("male")}
            >
              Mand
            </button>
            <button
              className={`border-button ${
                isFemaleFilter ? "bg-orange-200" : ""
              }`}
              onClick={() => handleFilterClick("female")}
            >
              Kvinde
            </button>
            <button
              className={`border-button ${
                isInternationalFilter ? "bg-orange-200" : ""
              }`}
              onClick={() => handleFilterClick("international")}
            >
              Internationalt
            </button>
          </div>
          <div>
            <select
              onChange={(e) => setSortMethod(e.target.value)}
              className="border-button"
            >
              <option value="name/asc">Sortér alfabetisk (Stigende)</option>
              <option value="name/desc">Sortér alfabetisk (Faldende)</option>
              <option value="likes/asc">Sortér efter likes (Stigende)</option>
              <option value="likes/desc">Sortér efter likes (Faldende)</option>
            </select>
          </div>
          {babyNames &&
            babyNames.map((babyName) => (
              <div
                key={babyName.id}
                className="flex items-center w-full justify-between"
              >
                <div className="flex items-center gap-2">
                  {babyName.isMale && babyName.isFemale ? (
                    <div className="flex items-center gap-0.5">
                      <MaleIcon />
                      <FemaleIcon />
                    </div>
                  ) : babyName.isMale ? (
                    <MaleIcon />
                  ) : (
                    <FemaleIcon />
                  )}

                  <p className="text-lg mr-2">{babyName.name}</p>
                </div>

                <div className="flex items-center">
                  {user &&
                  user.likedBabyNames &&
                  user.likedBabyNames.some(
                    (likedName) => likedName.id === babyName.id
                  ) ? (
                    <HeartFilledIcon
                      className="h-5 w-5 mr-1 text-rose-500 hover:text-rose-400 hover:cursor-pointer"
                      onClick={() => handleLikeClick(babyName)}
                    />
                  ) : (
                    <HeartIcon
                      className="h-5 w-5 mr-1 text-rose-500 hover:text-rose-400 hover:cursor-pointer"
                      onClick={() => handleLikeClick(babyName)}
                    />
                  )}

                  <p className="text-lg mr-1">{babyName.amountOfLikes} Likes</p>
                </div>
              </div>
            ))}
        </div>

        <div className="flex justify-between w-full gap-10 items-center">
          <button
            className="border-button"
            onClick={() => {
              handleBackClick();
            }}
          >
            Forrige
          </button>

          <p>{index}</p>

          <button
            className="border-button"
            onClick={() => {
              handleNextClick();
            }}
          >
            Næste
          </button>
        </div>
      </div>
    </div>
  );
}
