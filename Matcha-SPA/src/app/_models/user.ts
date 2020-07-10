import { Photo } from './photo';

export interface User {
  id: number;
  username: string;
  name: string;
  surname: string;
  age: number;
  gender: string;
  sexuality: string;
  created: Date;
  lastActive: any;
  photoUrl: string;
  city: string;
  country: string;
  password: string;
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  email: string;
  fameRating: number;
  deactivated: number;
  activated: number;
  token: string;
  reset: string;
  photos?: Photo[];
}
