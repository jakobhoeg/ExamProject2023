import { HeartFilledIcon, HeartIcon } from "@radix-ui/react-icons";
import { BabyName, User } from "../types/types";
import FemaleIcon from "./FemaleIcon";
import MaleIcon from "./MaleIcon";

interface Props {
  babyName: BabyName | null;
  user: User | null;
  handleLikeClick: (babyName: BabyName) => void;
}

const SwipeList: React.FC<Props> = ({ babyName, user }) => {
  return (
    <div className="w-[550px] pt-5">
      {babyName && (
        <div className="flex items-center justify-center">
          <div
            key={babyName.id}
            className="flex flex-col items-center p-12 border shadow-md"
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
              <p className="text-2xl mr-2">{babyName.name}</p>
            </div>

            <div className="flex items-center pt-2">
              {user &&
              user.likedBabyNames &&
              user.likedBabyNames.some(
                (likedName) => likedName.id === babyName.id
              ) ? (
                <HeartFilledIcon
                  className="h-5 w-5 mr-1 text-rose-500 hover:text-rose-400 "
                />
              ) : (
                <HeartIcon
                  className="h-5 w-5 mr-1 text-rose-500 hover:text-rose-400 "
                />
              )}
              <p className="text-lg mr-1">{babyName.amountOfLikes} Likes</p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default SwipeList;
