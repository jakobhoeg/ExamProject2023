import { useEffect, useState } from "react";
import "../../App.css";
import { HeartFilledIcon } from "@radix-ui/react-icons";

interface BabyName {
  id: string;
  name: string;
  isMale: boolean;
  isFemale: boolean;
  amountOfLikes: number;
}

export default function Names() {
  const [babyNames, setBabyNames] = useState<BabyName[]>([]);
  const [index, setIndex] = useState(1);
  const [isMaleFilter, setIsMaleFilter] = useState(false);
  const [isFemaleFilter, setIsFemaleFilter] = useState(false);
  const [isInternationalFilter, setIsInternationalFilter] = useState(false);

  const getBabyNames = async (index: number) => {
    try {
      const response = await fetch(
        `http://localhost:5000/babynames?page=${index}`,
        {
          method: "GET",
          credentials: "include",
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
      const url = new URL("http://localhost:5000/babynames/filter");
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
  }, [isMaleFilter, isFemaleFilter, isInternationalFilter]);

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
        break;
      case "female":
        setIsFemaleFilter(!isFemaleFilter);
        break;
      case "international":
        setIsInternationalFilter(!isInternationalFilter);
        break;
      default:
        break;
    }
  };

  return (
    <div className="flex flex-col w-screen pt-40 pb-20 justify-center items-center">
      <div className="flex flex-col justify-center items-center gap-8">
        <h1 className="text-5xl">Alle navne</h1>
        <div className="flex flex-col items-center justify-center w-96 gap-4">

          <div className="flex justify-center gap-4">
            <button
              className={`border-button ${isMaleFilter ? "bg-orange-200" : ""
                }`}
              onClick={() => handleFilterClick("male")}
            >
              Male
            </button>
            <button
              className={`border-button ${isFemaleFilter ? "bg-orange-200" : ""
                }`}
              onClick={() => handleFilterClick("female")}
            >
              Female
            </button>
            <button
              className={`border-button ${isInternationalFilter ? "bg-orange-200" : ""
                }`}
              onClick={() => handleFilterClick("international")}
            >
              International
            </button>
          </div>
          {babyNames &&
            babyNames.map((babyName) => (
              <div
                key={babyName.id}
                className="flex items-center w-full justify-between">
                <p className="mr-2">{babyName.name}</p>

                <div className="flex items-center">
                  <HeartFilledIcon className="h-4 w-4 mr-1 text-rose-500 hover:text-rose-400 hover:cursor-pointer" />
                  <p className="mr-1">{babyName.amountOfLikes} Likes</p>
                </div>


              </div>
            ))}
        </div>

        <div className="flex justify-between w-full gap-10 items-center">
          <button
            className="border-button"
            onClick={() => {
              handleBackClick()
            }}
          >
            Forrige
          </button>

          <p>{index}</p>

          <button
            className="border-button"
            onClick={() => {
              handleNextClick()
            }}
          >
            Næste
          </button>
        </div>
      </div>
    </div>
  );
}
