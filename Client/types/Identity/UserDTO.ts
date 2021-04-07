export interface UserGetDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface UserPostDTO {
  firstName: string;
  lastName: string;
  email: string;
  role: string | null;
}

export interface UserPatchDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

export interface UserPasswordPatchDTO {
  id: number;
  currentPassword: string;
  newPassword: string;
}

export interface UserRolePatchDTO {
  id: number;
  role: string;
}


