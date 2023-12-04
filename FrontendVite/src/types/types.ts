export interface User {
    id: string;
    firstName: string;
    email: string;
    partner: User | null;
  }

export interface BabyName {
    id: string;
    name: string;
    isMale: boolean;
    isFemale: boolean;
    isInternational: boolean;
    amountOfLikes: number;
  }

export interface MatchedBabyNames {
    id: string;
    users: User[];
    likedBabyNames: BabyName[] | null;
  }

