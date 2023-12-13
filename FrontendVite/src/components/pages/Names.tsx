import { useEffect, useState } from "react";
import "../../App.css";
import { BabyName, User } from "../../types/types";
import { useAuth } from "../../context/AuthProvider";
import NamesList from "../NamesList";
import SwipeList from "../SwipeList";
import { toast } from "sonner";

export default function Names() {
  const [babyNames, setBabyNames] = useState<BabyName[]>([]);
  const [index, setIndex] = useState(1);
  const [isMaleFilter, setIsMaleFilter] = useState(false);
  const [isFemaleFilter, setIsFemaleFilter] = useState(false);
  const [isInternationalFilter, setIsInternationalFilter] = useState(false);
  const [isSwipeMode, setSwipeMode] = useState(false);
  const [isListViewMode, setListViewMode] = useState(true);
  const [sortMethod, setSortMethod] = useState("name/asc");
  const [lastLikedNameId, setLastLikedNameId] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);
  const { isLoggedIn } = useAuth();
  const [randomBabyName, setRandomBabyName] = useState<BabyName | null>(null);

  //#region Functions
  useEffect(() => {
    const getUserInfo = async () => {
      try {
        const response = await fetch("http://51.20.73.95:5000/api/user", {
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
      setLastLikedNameId(null);
    }
  }, [isLoggedIn, lastLikedNameId]);

  function formatNumber(num) {
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'k';
    } else {
      return num.toString();
    }
  }

  const getBabyData = async (index: number, isFiltering = false) => {
    try {
      const url = isFiltering
        ? new URL(`http://51.20.73.95:5000/api/babynames/sort/${sortMethod}`)
        : new URL(`http://51.20.73.95:5000/api/babynames/?page=${index}`);

      if (isFiltering) {
        url.searchParams.append("page", index.toString());
        url.searchParams.append("isMale", isMaleFilter.toString());
        url.searchParams.append("isFemale", isFemaleFilter.toString());
        url.searchParams.append(
          "isInternational",
          isInternationalFilter.toString()
        );
      }

      const response = await fetch(url.toString(), {
        method: "GET",
        credentials: "include",
      });

      if (response.ok) {
        const babyNameData = await response.json();
        setBabyNames(babyNameData);
      } else {
        throw new Error("Name not found");
      }
    } catch (error) {
      console.error(error);
    }
  };

  const getRandomBabyName = async (isFiltering = false) => {
    try {
      let url = new URL("http://51.20.73.95:5000/api/babyname/random");

      if (isSwipeMode && isFiltering) {
        // Include sorting criteria if in swipemode
        url = new URL(`http://51.20.73.95:5000/api/babyname/random/sort`);
        url.searchParams.append("sortMethod", sortMethod);
        url.searchParams.append("isMale", isMaleFilter.toString());
        url.searchParams.append("isFemale", isFemaleFilter.toString());
        url.searchParams.append(
          "isInternational",
          isInternationalFilter.toString()
        );
      }

      const response = await fetch(url.toString(), {
        method: "GET",
        credentials: "include",
      });

      if (response.ok) {
        const babyNameData = await response.json();
        setRandomBabyName(babyNameData);
      } else {
        throw new Error("Name not found");
      }
    } catch (error) {
      console.error(error);
    }
  };

  const checkFiltersAndFetchNames = () => {
    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyData(index, true);
    } else {
      getBabyData(index);
    }
  };

  useEffect(() => {
    const url = new URL(window.location.href);
    const pageIndex = url.searchParams.get("page");
    setIndex(Number(pageIndex) || 1);

    if (isSwipeMode) {
      if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
        getRandomBabyName(true);
      } else {
        getRandomBabyName();
      }
    } else {
      if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
        getBabyData(Number(pageIndex) || 1, true);
      } else {
        getBabyData(Number(pageIndex) || 1);
      }
    }
  }, [
    isSwipeMode,
    isMaleFilter,
    isFemaleFilter,
    isInternationalFilter,
    sortMethod,
  ]);

  //#endregion

  //#region Event handling
  const handlePageClick = (newIndex: number) => {
    setIndex(newIndex);
    window.history.pushState({}, "", `/navne?page=${newIndex}`);

    if (isMaleFilter || isFemaleFilter || isInternationalFilter) {
      getBabyData(newIndex, true);
    } else {
      getBabyData(newIndex);
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



  const handleLikeClick = async (babyName: BabyName) => {
    try {
      const response = await fetch(`http://51.20.73.95:5000/api/babynames/like`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(babyName),
        credentials: "include",
      });

      if (response.ok) {
        // Update state to toggle UI immediately
        setLastLikedNameId(babyName.id);

        // Reset local state to null after a short delay
        setLastLikedNameId(null);

        // Fetchi the latest list of baby names (with updated likes)
        checkFiltersAndFetchNames();
      } else {
        // If the API call fails, handle the error appropriately
        toast.error("Du skal være logget ind for at like navne");
        throw new Error("Something went wrong");
      }
    } catch (error) {
      console.error(error);
    }
  };
  //#endregion

  //#region HTML
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
            onClick={() => {
              handleViewClick("swipe");
              getRandomBabyName();
            }}
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
              className={`border-button ${isFemaleFilter ? "bg-orange-200" : ""
                }`}
              onClick={() => handleFilterClick("female")}
            >
              Kvinde
            </button>
            <button
              className={`border-button ${isInternationalFilter ? "bg-orange-200" : ""
                }`}
              onClick={() => handleFilterClick("international")}
            >
              Internationalt
            </button>
          </div>
          {isListViewMode && (
            <div>
              <select
                onChange={(e) => setSortMethod(e.target.value)}
                className="border-button"
              >
                <option value="name/asc">Sortér alfabetisk (Stigende)</option>
                <option value="name/desc">Sortér alfabetisk (Faldende)</option>
                <option value="likes/asc">Sortér efter likes (Stigende)</option>
                <option value="likes/desc">
                  Sortér efter likes (Faldende)
                </option>
              </select>
            </div>
          )}
          <div className="flex justify-center gap-4 pt-10 pb-10">
            <label htmlFor="SearchLabel">Søg efter navn:</label>
            <input
              type="searchBox"
              name="searchFunc"
              className="inputs "
            />
            <button className="border-button">
              Søg
            </button>
          </div>

          {/* List of names */}
          {isListViewMode && (
            <NamesList
              babyNames={babyNames}
              user={user}
              handleLikeClick={handleLikeClick}
            />
          )}

          {/* Swipe mode */}
          {isSwipeMode && (
            <SwipeList
              babyName={randomBabyName}
              user={user}
              handleLikeClick={handleLikeClick}
            />
          )}
        </div>
        {/* List mode buttons */}
        {isListViewMode && (
          <div className="flex justify-between w-full gap-10 items-center">
            <button
              className="border-button"
              onClick={() => handlePageClick(index - 1)}
            >
              Forrige
            </button>
            <p>{index}</p>
            <button
              className="border-button"
              onClick={() => handlePageClick(index + 1)}
            >
              Næste
            </button>
          </div>
        )}

        {/* Swipe buttons */}
        {isSwipeMode && (
          <div className="flex w-1/2 justify-between">
            <button
              className="border-button"
              onClick={() => {
                getRandomBabyName(
                  isMaleFilter || isFemaleFilter || isInternationalFilter
                );
              }}
            >
              Skip
            </button>
            <button
              className="border-button"
              onClick={() => {
                getRandomBabyName(
                  isMaleFilter || isFemaleFilter || isInternationalFilter
                );
                handleLikeClick(randomBabyName!);
              }}
            >
              Like
            </button>
          </div>
        )}
      </div>
    </div>
  );
  //#endregion
}
