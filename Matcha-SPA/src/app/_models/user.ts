import { Photo } from './photo';

export interface User {
  id: number;
  username: string;
  name: string;
  surname: string;
  age: number;
  gender: string;
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
  photos?: Photo[];
}
