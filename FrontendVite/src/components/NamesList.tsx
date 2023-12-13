import React from "react";
import { BabyName, User } from "../types/types";
import MaleIcon from "./MaleIcon";
import FemaleIcon from "./FemaleIcon";
import { HeartFilledIcon, HeartIcon } from "@radix-ui/react-icons";

interface Props {
  babyNames: BabyName[] | undefined;
  user: User | null;
  handleLikeClick: (babyName: BabyName) => void;
}
const formatLikes = (likes: number): string => {
  return likes < 1000 ? likes.toString() : `${(likes / 1000).toFixed(1)}k`;
};

const NamesList: React.FC<Props> = ({ babyNames, user, handleLikeClick }) => {
  return (
    <div className="w-[550px]">
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
              <p className="text-lg mr-1">{formatLikes(babyName.amountOfLikes)} Likes</p>
            </div>
          </div>
        ))}
    </div>
  );
};

export default NamesList;
