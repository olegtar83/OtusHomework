import { Friend } from '../types/Friend';

export const transformToFriend = (data: any): Friend => {
  return {
    id: data.id,
    firstName: data.first_name,
    secondName: data.second_name,
    city: data.city,
  };
};