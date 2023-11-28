import { useEffect, useState } from "react";
import "../../App.css";
import { HeartIcon } from "@radix-ui/react-icons";

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

    const getBabyNames = async (index: number) => {
        try {
            const response = await fetch(`http://localhost:5000/babynames?page=${index}`, {
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

        getBabyNames(Number(pageIndex) || 1);
    }, []);

    const checkIndex = () => {
        if (index > 1) {
            setIndex(index-1);
            window.history.pushState({}, "", `/navne?page=${index - 1}`);
        }
        else {
            setIndex(1);
            window.history.pushState({}, "", `/navne?page=${1}`);
        }
    }


  return (
     <div className="flex flex-col w-screen pt-40 pb-20 justify-center items-center">
        <div className="flex flex-col justify-center items-center">
        <h1 className="text-5xl">
            Alle navne
        </h1>
        <div className="flex flex-col items-center justify-center" >
        {babyNames && babyNames.map(babyName => 
            <div key={babyName.id} className="flex items-center ">
                <p className="mr-2">{babyName.name}</p>
                <div className="flex items-center">
                <HeartIcon className="h-4 w-4 mr-1"/>
                <p>{babyName.amountOfLikes}</p>
            </div>
            </div>
        )}
        
        <div className="flex justify-between w-full gap-10 items-center">
        <button className="border-button"
            onClick={() => {
                checkIndex();
                getBabyNames(index - 1);
            }}
            >Forrige</button>

            <p>{index}</p>

            <button className="border-button"
            onClick={() => {
                setIndex(index + 1);
                window.history.pushState({}, "", `/navne?page=${index + 1}`);
                getBabyNames(index + 1);
            }}
            >Næste</button>
        </div>
        </div>
        </div>
    </div>
  )
}
